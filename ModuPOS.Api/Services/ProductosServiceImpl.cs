using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
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

        //CREAR PRODUCTO
        public async Task<ProductoResponse> CrearProductoAsync(
            CrearProductoRequest request,
            IFormFile? archivoImagen,
            IImagenService imagenService)
        {
            if (await _db.Productos.AnyAsync(p => p.SKU == request.SKU))
                throw new InvalidOperationException($"Ya existe un producto con el SKU '{request.SKU}'.");

            Imagen? imagen = null;
            if (archivoImagen is { Length: > 0 })
            {
                //si hay imagen, primer la subimos a cloudinary
                await using var stream = archivoImagen.OpenReadStream();
                var subida = await imagenService.SubirAsync(stream, archivoImagen.FileName);

                //guardar db
                imagen = new Imagen
                {
                    Url = subida.Url,
                    Nombre = archivoImagen.FileName,
                    ProveedorId = subida.PublicId,
                    Proveedor = "Cloudinary"
                };

                _db.Imagenes.Add(imagen);
                await _db.SaveChangesAsync();
            }

            var producto = new Producto
            {
                SKU = request.SKU.Trim().ToUpperInvariant(),
                Nombre = request.Nombre.Trim(),
                PrecioActual = request.PrecioActual,
                Stock = request.Stock,
                CategoriaId = request.CategoriaId,
                ImagenId = imagen?.Id
            };

            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            return await ObtenerProductoPorIdAsync(producto.Id)
               ?? throw new InvalidOperationException("Error al recuperar el producto creado.");
        }

        //ACTUALIZAR PRODUCTO
        public async Task<ProductoResponse?> ActualizarProductoAsync(
            ActualizarProductoRequest request,
            IFormFile? archivoImagen,
            IImagenService imagenService)
        {
            var producto = await _db.Productos
                .Include(p => p.Imagen)
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            if (producto is null) return null;

            if (request.SKU is not null && request.SKU != producto.SKU)
            {
                bool skuEnUso = await _db.Productos.AnyAsync(p => p.SKU == request.SKU && p.Id != request.Id);
                if (skuEnUso) throw new InvalidOperationException($"Ya existe un producto con el SKU '{request.SKU}'.");
            }

            producto.SKU = request.SKU?.Trim().ToUpperInvariant() ?? producto.SKU;
            producto.Nombre = request.Nombre?.Trim() ?? producto.Nombre;
            producto.PrecioActual = request.PrecioActual ?? producto.PrecioActual;
            producto.Stock = request.Stock ?? producto.Stock;
            producto.CategoriaId = request.CategoriaId ?? producto.CategoriaId;

            //quitar imagen o reemplazar imagen
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

            //se esta subiendo una nueva imagen
            if (archivoImagen is { Length: > 0 })
            {
                await using var stream = archivoImagen.OpenReadStream();
                var subida = await imagenService.SubirAsync(stream, archivoImagen.FileName);

                var nuevaImagen = new Imagen
                {
                    Url = subida.Url,
                    Nombre = archivoImagen.FileName,
                    ProveedorId = subida.PublicId,
                    Proveedor = "Cloudinary"
                };

                _db.Imagenes.Add(nuevaImagen);
                await _db.SaveChangesAsync();

                producto.ImagenId = nuevaImagen.Id; //asignación de la imagen recien subida
            }

            await _db.SaveChangesAsync();
            return await ObtenerProductoPorIdAsync(producto.Id);
        }

        public async Task<List<ProductoResponse>> ObtenerProductosAsync()
        {
            return await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .Select(p => MapToResponse(p))
                .ToListAsync();
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto is null) return false;

            producto.IsDeleted = true;
            producto.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductoResponse>> BuscarProductosAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino)) return new List<ProductoResponse>();

            var patron = $"%{termino.Trim()}%";

            return await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .Where(p => EF.Functions.Like(p.Nombre, patron)
                            || EF.Functions.Like(p.SKU, patron))
                .Select(p => MapToResponse(p))
                .ToListAsync();
        }

        public async Task<ProductoResponse?> ObtenerProductoPorIdAsync(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .FirstOrDefaultAsync(p => p.Id == id);

            return producto is null ? null : MapToResponse(producto);
        }

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

        public async Task<ProductoResponse> AjustarStockAsync(AjusteStockRequest request)
        {
            var producto = await _db.Productos.FindAsync(request.ProductoId);

            if (producto == null) throw new InvalidOperationException("$No se encontró el producto con ID {request.ProductoId}");

            producto.Stock += request.Cantidad;

            //no stock negativo
            if (producto.Stock < 0) throw new InvalidOperationException($"El ajuste resultaría en un stock negativo para '{producto.Nombre}'.");

            await _db.SaveChangesAsync();
            return MapToResponse(producto);
        }
    }
}
