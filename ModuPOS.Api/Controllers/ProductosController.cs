using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Api.Services;
using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductosService _productosService;

        public ProductosController(IProductosService productosService)
        {
            _productosService = productosService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoResponse>> CrearProducto(
            [FromBody] CrearProductoRequest request)
        {
            var response = await _productosService.CrearProductoAsync(request);
            return CreatedAtAction(nameof(ObtenerProductos), response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProductoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductoResponse>> ObtenerProductos()
        {
            var productos = await _productosService.ObtenerProductosAsync();
            return Ok(productos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoResponse>> ObtenerProducto(int id)
        {
            var producto = await _productosService.ObtenerProductoPorIdAsync(id);
            return producto is null ? NotFound() : Ok(producto);
        }

        [HttpPatch]
        [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoResponse>> ActualizarProducto(
            [FromBody] ActualizarProductoRequest request)
        {
            var response = await _productosService.ActualizarProductoAsync(request);
            return response is null ? NotFound() : Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarProductoAsync(int id)
        {
            var eliminado = await _productosService.EliminarProductoAsync(id);

            return eliminado ? NoContent() : NotFound();
        }

        [HttpGet("buscar")]
        [ProducesResponseType(typeof(List<ProductoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ProductoResponse>>> BuscarProductos(
            [FromQuery] string termino)
        { 
            if (string.IsNullOrWhiteSpace(termino)) return BadRequest("El término de búsqueda no puede estar vacío.");

            var resultados = await _productosService.BuscarProductosAsync(termino);
            return Ok(resultados);
        }

        [HttpPatch("ajustar-stock")]
        [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoResponse>> AjustarStock([FromBody] AjusteStockRequest request)
        {
            var response = await _productosService.AjustarStockAsync(request);
            return Ok(response);
        }
    }
}
