using Microsoft.AspNetCore.Mvc;
using ModuPOS.Api.Services;
using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetodosPagoController : ControllerBase
    {
        private readonly IMetodosPagoService _service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetodoPagoResponse>>> Get()
        {
            return Ok(await _service.ObtenerMetodosPagoAsync());
        }

        [HttpPost]
        public async Task<ActionResult<MetodoPagoResponse>> Post([FromBody] CrearMetodoPagoRequest request)
        {
            try
            {
                var resultado = await _service.CrearMetodoPagoAsync(request);
                return CreatedAtAction(nameof(Get), new { id = resultado.Id }, resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
