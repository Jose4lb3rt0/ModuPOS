using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Services
{
    public class ProductosServiceImpl : IProductosService
    {
        private readonly ModuPosDbContext _db;

        public ProductosServiceImpl(ModuPosDbContext db)
        {
            _db = db;
        }

        public async Task<ProductoResponse?> ActualizarProductoAsync(ActualizarProductoRequest request)
        {
            var producto = await _db.Productos.FindAsync(request.Id);
            if (producto is null) return null;

            if (request.SKU is not null && request.SKU != producto.SKU)
            {
                bool skuEnUso = await _db.Productos
                    .AnyAsync(p => p.SKU == request.SKU && p.Id != request.Id);

                if (skuEnUso)
                    throw new InvalidOperationException($"Ya existe un producto con el SKU '{request.SKU}'.");
            }

            producto.SKU = request.SKU?.Trim().ToUpperInvariant() ?? producto.SKU;
            producto.Nombre = request.Nombre?.Trim() ?? producto.Nombre;
            producto.PrecioActual = request.PrecioActual ?? producto.PrecioActual;
            producto.Stock = request.Stock ?? producto.Stock;

            await _db.SaveChangesAsync();

            return MapToResponse(producto);
        }

        public async Task<ProductoResponse> CrearProductoAsync(CrearProductoRequest request)
        {
            if (await _db.Productos.AnyAsync(p => p.SKU == request.SKU))
                throw new InvalidOperationException($"Ya existe un producto con el SKU '{request.SKU}'.");

            var producto = new Producto
            {
                SKU = request.SKU.Trim().ToUpperInvariant(),
                Nombre = request.Nombre.Trim(),
                PrecioActual = request.PrecioActual,
                Stock = request.Stock,
            };

            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            return MapToResponse(producto);
        }

        public async Task<List<ProductoResponse>> ObtenerProductosAsync()
        {
            return await _db.Productos
            .Select(p => new ProductoResponse
            {
                Id = p.Id,
                SKU = p.SKU,
                Nombre = p.Nombre,
                PrecioActual = p.PrecioActual,
                Stock = p.Stock
            })
            .ToListAsync();
        }

        private static ProductoResponse MapToResponse(Producto p) => new()
        {
            Id = p.Id,
            SKU = p.SKU,
            Nombre = p.Nombre,
            PrecioActual = p.PrecioActual,
            Stock = p.Stock,
        };
    }
}
