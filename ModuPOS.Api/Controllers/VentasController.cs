using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.Enums;

namespace ModuPOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly ModuPosDbContext _db;
        private const decimal tasa_impuesto = 0.16m;

        public VentasController(ModuPosDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<VentaResponse>> RegistrarVenta(
            [FromBody] RegistrarVentaRequest request)
        {
            //validacion
            if (!request.Items.Any()) return BadRequest("La venta debe contener al menos un item.");

            //query db
            var productoIds = request.Items.Select(i => i.ProductoId).ToList();
            var productos = await _db.Productos
                .Where(p => productoIds.Contains(p.Id))
                .ToListAsync();

            //verificar productos descontinuados¿
            var productosEliminados = await _db.Productos
                .IgnoreQueryFilters()          // Saltamos el filtro global solo aquí
                .Where(p => productoIds.Contains(p.Id) && p.IsDeleted)
                .Select(p => p.Nombre)
                .ToListAsync();

            if (productosEliminados.Any())
                return BadRequest(
                    $"Los siguientes productos fueron descontinuados y no pueden venderse: " +
                    $"{string.Join(", ", productosEliminados)}");

            //verificar existencia
            if (productos.Count != productoIds.Distinct().Count()) return BadRequest("Uno o más productos no existen.");

            //transacción explícita
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var detalles = new List<VentaDetalle>();
                decimal subtotal = 0;

                foreach (var item in request.Items)
                {
                    var producto = productos.First(p => p.Id == item.ProductoId);

                    //no stock
                    if (producto.Stock < item.Cantidad)
                    {
                        return BadRequest(
                            $"Stock insuficiente para '{producto.Nombre}'. " +
                            $"Disponible: {producto.Stock}, Solicitado: {item.Cantidad}");
                    }

                    //si stock, descontar
                    producto.Stock -= item.Cantidad;

                    //precio historico
                    var detalle = new VentaDetalle()
                    {
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitarioHistorico = producto.PrecioActual //tal y como está hoy, venta intacta si el precio cambia
                    };

                    detalles.Add(detalle);
                    subtotal += detalle.Cantidad * detalle.PrecioUnitarioHistorico;
                }

                //calcular totales
                decimal impuestos = Math.Round(subtotal * tasa_impuesto, 4);
                decimal descuento = Math.Round(request.DescuentoTotal, 4);

                //venta
                var venta = new Venta()
                {
                    Folio = request.Folio,
                    Fecha = DateTime.UtcNow,
                    MetodoPagoId = request.MetodoPagoId,
                    Subtotal = subtotal,
                    Impuestos = impuestos,
                    DescuentoTotal = descuento,
                    Estado = EstadoVenta.Completada,
                    Detalles = detalles
                };

                //persistir
                _db.Ventas.Add(venta);
                await _db.SaveChangesAsync();

                //commit
                await transaction.CommitAsync();

                //response
                var ventaGuardada = await _db.Ventas
                    .Include(v => v.Detalles)
                        .ThenInclude(d => d.Producto)
                    .FirstAsync(v => v.Id == venta.Id);

                var response = MapToResponse(ventaGuardada);
                return CreatedAtAction(
                    nameof(ObtenerVenta),
                    new { id = venta.Id, response }
                );

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno al registrar la venta: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VentaResponse>> ObtenerVenta(int id)
        {
            var venta = await _db.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta is null) return NotFound("Venta no encontrada.");

            return Ok(MapToResponse(venta));
        }

        private static VentaResponse MapToResponse(Venta venta) => new()
        {
            Id = venta.Id,
            Folio = venta.Folio,
            Fecha = venta.Fecha,
            Subtotal = venta.Subtotal,
            Impuestos = venta.Impuestos,
            DescuentoTotal = venta.DescuentoTotal,
            Total = venta.Total,
            Estado = venta.Estado.ToString(),
            Detalles = venta.Detalles.Select(d => new DetalleVentaResponse
            {
                NombreProducto = d.Producto.Nombre,
                Cantidad = d.Cantidad,
                PrecioUnitarioHistorico = d.PrecioUnitarioHistorico,
                SubtotalLinea = d.SubtotalLinea
            }).ToList()
        };
    }

}
