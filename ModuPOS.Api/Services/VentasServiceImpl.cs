using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.Enums;

namespace ModuPOS.Api.Services
{
    public class VentasServiceImpl : IVentasService
    {
        private readonly ModuPosDbContext _db;
        private const decimal tasa_impuesto = 0.16m;

        public VentasServiceImpl(ModuPosDbContext db)
        {
            _db = db;
        }

        public async Task<VentaResult> RegistrarVentaAsync(RegistrarVentaRequest request)
        {
            //nulo
            if (request.Items is null || request.Items.Count == 0) return VentaResult.Fail("La venta debe contener al menos un producto.");

            //carga db - el global query filter ya excluye isDeleted por BaseEntity en el contexto
            var productoIds = request.Items.Select(i => i.ProductoId).Distinct().ToList();
            var productos = await _db.Productos
                .Where(p => productoIds.Contains(p.Id))
                .ToListAsync();

            //validar existencia
            if (productos.Count != productoIds.Count)
            {
                var encontradosIds = productos.Select(p => p.Id);
                var faltantes = productoIds.Except(encontradosIds);

                return VentaResult.Fail($"Productos no encontrados o descontinuados: {string.Join(", ", faltantes)}");
            }

            //validar stock
            var erroresStock = new List<string>();

            foreach (var item in request.Items)
            {
                var producto = productos.First(p => p.Id == item.ProductoId);

                if (item.Cantidad <= 0)
                    erroresStock.Add($"'{producto.Nombre}': la cantidad debe ser mayor a cero.");

                else if (producto.Stock < item.Cantidad)
                    erroresStock.Add(
                        $"'{producto.Nombre}': stock insuficiente " +
                        $"(Disponible: {producto.Stock}; Solicitado: {item.Cantidad}).");
            }

            if (erroresStock.Count > 0)
                return VentaResult.Fail(string.Join(" | ", erroresStock)); //ver errores

            //transacción
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var detalles = new List<VentaDetalle>();
                decimal subtotal = 0m;

                foreach (var item in request.Items)
                {
                    var producto = productos.First(p => p.Id == item.ProductoId);

                    var detalle = new VentaDetalle()
                    {
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitarioHistorico = producto.PrecioActual,
                        //subtotallinea columna calculada
                    };

                    detalles.Add(detalle);

                    subtotal += item.Cantidad * producto.PrecioActual;
                    producto.Stock -= item.Cantidad; //ahorita es trackeo -> SaveChangesAsync actualiza
                }

                decimal impuestos = Math.Round(subtotal * tasa_impuesto, 4, MidpointRounding.AwayFromZero);
                decimal descuento = Math.Round(request.DescuentoTotal, 4, MidpointRounding.AwayFromZero);

                //venta
                var venta = new Venta()
                {
                    Folio = request.Folio.Trim().ToUpperInvariant(),
                    Fecha = DateTime.UtcNow,
                    MetodoPagoId = request.MetodoPagoId,
                    Subtotal = subtotal,
                    Impuestos = impuestos,
                    DescuentoTotal = descuento,
                    Estado = EstadoVenta.Completada, //seteo manual
                    Detalles = detalles
                };

                //persistir
                _db.Ventas.Add(venta);
                await _db.SaveChangesAsync();

                //commit
                await transaction.CommitAsync();

                //response (+ columnas calculadas)
                var ventaGuardada = await _db.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstAsync(v => v.Id == venta.Id);

                return VentaResult.Ok(MapToResponse(ventaGuardada));
            } 
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                //ilogger en producción
                Console.Error.WriteLine($"[VentasService] Error al registrar venta: {ex}");

                return VentaResult.Fail("Ocurrió un error interno al registrar la venta.");
            }
        }

        public async Task<VentaResponse?> ObtenerVentaAsync(int id)
        {
            var venta = await _db.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            return venta is null ? null : MapToResponse(venta);
        }

        private static VentaResponse MapToResponse(Venta venta) => new()
        {
            Id = venta.Id,
            Folio = venta.Folio,
            Fecha = venta.Fecha,
            Subtotal = venta.Subtotal,
            Impuestos = venta.Impuestos,
            DescuentoTotal = venta.DescuentoTotal,
            Total = venta.Total, //columna computed
            Estado = venta.Estado.ToString(),
            Detalles = venta.Detalles.Select(d => new DetalleVentaResponse
            {
                NombreProducto = d.Producto.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitarioHistorico = d.PrecioUnitarioHistorico,
                SubtotalLinea = d.SubtotalLinea //columna computed
            }).ToList()
        };
    }
}

