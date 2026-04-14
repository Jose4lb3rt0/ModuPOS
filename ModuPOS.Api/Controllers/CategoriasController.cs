using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuPOS.Api.Services;
using ModuPOS.Shared.DTOs.Categoria;

namespace ModuPOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriasService _service;

        public CategoriasController(ICategoriasService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CategoriaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoriaResponse>>> ObtenerTodas()
            => Ok(await _service.ObtenerCategoriasAsync());

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponse>> ObtenerPorId(int id)
        {
            var resultado = await _service.ObtenerCategoriaAsync(id);
            return resultado is null ? NotFound() : Ok(resultado);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoriaResponse>> Crear(
        [FromBody] CrearCategoriaRequest request)
        {
            var resultado = await _service.CrearCategoriaAsync(request);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
        }

        [HttpPatch]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponse>> Actualizar(
        [FromBody] ActualizarCategoriaRequest request)
        {
            var resultado = await _service.ActualizarCategoriaAsync(request);
            return resultado is null ? NotFound() : Ok(resultado);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarCategoriaAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}
