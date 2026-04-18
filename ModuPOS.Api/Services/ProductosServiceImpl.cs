using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Categoria;
using ModuPOS.Shared.DTOs.Imagen;
using ModuPOS.Shared.DTOs.Producto;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ModuPOS.Api.Services
{
    public class ProductosServiceImpl : IProductosService
    {
        private readonly ModuPosDbContext _db;

        public ProductosServiceImpl(ModuPosDbContext db)
        {
            _db = db;
        }

        //1. CREAR
        public async Task<ProductoResponse> CrearProductoAsync(
            CrearProductoRequest request,
            IFormFile? archivoImagen,
            IImagenService imagenService)
        {
            //sku
            var sku = request.SKU.Trim().ToUpperInvariant();
            if (await _db.Productos.AnyAsync(p => p.SKU == sku))
                throw new InvalidOperationException($"Ya existe un producto con el SKU '{sku}'.");

            //categoria
            var categoriaId = request.CategoriaId <= 0 ? null : request.CategoriaId;

            //imagen
            Imagen? imagen = await imagenService.SubirYPersistirAsync(archivoImagen, _db);

            var producto = new Producto
            {
                SKU = sku,
                Nombre = request.Nombre.Trim(),
                PrecioActual = request.PrecioActual,
                Stock = request.Stock,
                CategoriaId = categoriaId,
                ImagenId = imagen?.Id
            };

            _db.Productos.Add(producto);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch
            {
                if (imagen is not null)
                    await imagenService.EliminarAsync(imagen.ProveedorId);
                throw;
            }

            return await ObtenerProductoPorIdAsync(producto.Id)
               ?? throw new InvalidOperationException("Error al recuperar el producto creado.");
        }

        //2. ACTUALIZAR
        public async Task<ProductoResponse?> ActualizarProductoAsync(
            ActualizarProductoRequest request,
            IFormFile? archivoImagen,
            IImagenService imagenService)
        {
            //find
            var producto = await _db.Productos
                .Include(p => p.Imagen)
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            if (producto is null) return null;

            //sku
            if (request.SKU is not null)
            {
                var sku = request.SKU?.Trim().ToUpperInvariant();
                if (sku != producto.SKU) 
                { 
                    bool skuEnUso = await _db.Productos.AnyAsync(p => p.SKU == sku && p.Id != request.Id);
                    if (skuEnUso) throw new InvalidOperationException($"Ya existe un producto con el SKU '{sku}'.");
                    producto.SKU = sku!;
                }
            }

            producto.Nombre = request.Nombre?.Trim() ?? producto.Nombre;
            producto.PrecioActual = request.PrecioActual ?? producto.PrecioActual;
            producto.Stock = request.Stock ?? producto.Stock;
            producto.CategoriaId = request.CategoriaId <0 ? null : request.CategoriaId ?? producto.CategoriaId;

            //imagen: quitar imagen o reemplazar imagen
            if (request.QuitarImagen || (archivoImagen is { Length: > 0}))
            {
                if (producto.Imagen != null)
                {
                    await imagenService.EliminarAsync(producto.Imagen.ProveedorId);
                    _db.Imagenes.Remove(producto.Imagen);
                    producto.ImagenId = null;
                }
                //la subida esta en el bloque siguiente
            }

            if (archivoImagen is { Length: > 0 })
            {
                var nuevaImagen = await imagenService.SubirYPersistirAsync(archivoImagen, _db);
                if (nuevaImagen is not null) producto.ImagenId = nuevaImagen.Id; //asignación de la imagen recien subida
            }

            await _db.SaveChangesAsync();
            return await ObtenerProductoPorIdAsync(producto.Id);
        }

        //3. OBTENER TODOS CON PAGINACION
        public async Task<PagedResponse<ProductoResponse>> ObtenerProductosAsync(int pageIndex, int pageSize)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            pageIndex = Math.Max(pageIndex, 0);

            var total = await _db.Productos.CountAsync();

            var items = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .OrderBy(p => p.Nombre) //orden determinista
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(p => MapToResponse(p))
                .ToListAsync();

            return PagedResponse<ProductoResponse>.Montar(items, total, pageIndex, pageSize);
        }

        //4. OBTENER POR ID
        public async Task<ProductoResponse?> ObtenerProductoPorIdAsync(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .FirstOrDefaultAsync(p => p.Id == id);

            return producto is null ? null : MapToResponse(producto);
        }

        //5. ELIMINAR
        public async Task<bool> EliminarProductoAsync(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto is null) return false;

            producto.IsDeleted = true;
            producto.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        //6. STOCK
        public async Task<ProductoResponse> AjustarStockAsync(AjusteStockRequest request)
        {
            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .FirstOrDefaultAsync(p => p.Id == request.ProductoId);

            if (producto == null) throw new InvalidOperationException($"No se encontró el producto con ID {request.ProductoId}");

            producto.Stock += request.Cantidad;

            //no stock negativo
            if (producto.Stock < 0)
                throw new InvalidOperationException(
                    $"El ajuste resultaría en stock negativo para '{producto.Nombre}'. " +
                    $"Stock actual: {producto.Stock - request.Cantidad}, ajuste: {request.Cantidad}.");

            await _db.SaveChangesAsync();
            return MapToResponse(producto);
        }

        //7. BUSCAR CON FILTROS Y PAGINACION
        public async Task<PagedResponse<ProductoResponse>> BuscarProductosAsync(BuscarProductosRequest request)
        {
            request.PageSize = Math.Clamp(request.PageSize, 1, 100);
            request.PageIndex = Math.Max(request.PageIndex, 0);

            var categoriaId = request.CategoriaId <= 0 ? null : request.CategoriaId;

            var query = _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .AsQueryable(); //no estoy ejecutando asincronamente

            if (!string.IsNullOrWhiteSpace(request.Termino))
            { 
                var patron = $"%{request.Termino.Trim()}%";

                query = query.Where(p =>
                    EF.Functions.Like(p.Nombre, patron) ||
                    EF.Functions.Like(p.SKU, patron));
            }

            //filtro por categoria
            if (categoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == categoriaId.Value); //anidacion de query

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Nombre)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(p => MapToResponse(p))
                .ToListAsync();

            return PagedResponse<ProductoResponse>.Montar(items, total, request.PageIndex, request.PageSize);
        }

        //MAPPER ENTIDAD A DTO
        private static ProductoResponse MapToResponse(Producto p) => new(
            Id: p.Id,
            SKU: p.SKU,
            Nombre: p.Nombre,
            PrecioActual: p.PrecioActual,
            Stock: p.Stock,
            Categoria: p.Categoria is null 
                ? null
                : new CategoriaResumenResponse(
                    p.Categoria.Id, 
                    p.Categoria.Nombre, 
                    p.Categoria.Color),
            Imagen: p.Imagen is null ? null
                            : new ImagenResponse(p.Imagen.Id, p.Imagen.Url, p.Imagen.Nombre)
        );
    }
}
