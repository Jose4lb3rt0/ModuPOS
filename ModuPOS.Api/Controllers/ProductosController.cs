using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ModuPosDbContext _db;

        public ProductosController(ModuPosDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<ProductoResponse>> CrearProducto(
            [FromBody] CrearProductoRequest request)
        {
            if (await _db.Productos.AnyAsync(p => p.SKU == request.SKU)) return BadRequest($"Ya existe un producto con este SKU.");

            var producto = new Producto()
            {
                SKU = request.SKU,
                Nombre = request.Nombre,
                PrecioActual = request.PrecioActual,
                Stock = request.Stock,
            };

            //persistir
            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            //response
            return Ok(MapToResponse(producto));
        }

        [HttpGet]
        public async Task<ActionResult<ProductoResponse>> ObtenerProductos()
        {
            var productos = await _db.Productos
                .Select(p => new ProductoResponse()
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Nombre = p.Nombre,
                    PrecioActual = p.PrecioActual,
                    Stock = p.Stock
                }).ToListAsync();

            return Ok(productos);
        }

        [HttpPatch]
        public async Task<ActionResult<ProductoResponse>> ActualizarProducto(
            [FromBody] ActualizarProductoRequest request)
        {
            var producto = await _db.Productos.FindAsync(request.Id);
            if (producto == null) return NotFound($"No se encontró el producto con id {request.Id}.");

            if (request.SKU != null)
            { 
                if (await _db.Productos.AnyAsync(p => p.SKU == request.SKU && p.Id != request.Id)) 
                    return BadRequest($"Ya existe un producto con el SKU {request.SKU}.");

                producto.SKU = request.SKU;
            }

            if (request.Nombre != null) producto.Nombre = request.Nombre;
            if (request.PrecioActual != null) producto.PrecioActual = request.PrecioActual.Value;
            if (request.Stock != null) producto.Stock = request.Stock.Value;

            await _db.SaveChangesAsync();

            return Ok(MapToResponse(producto));
        }

        public static ProductoResponse MapToResponse(Producto producto) => new()
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            PrecioActual = producto.PrecioActual,
            SKU = producto.SKU,
            Stock = producto.Stock,
        };
    }
}
