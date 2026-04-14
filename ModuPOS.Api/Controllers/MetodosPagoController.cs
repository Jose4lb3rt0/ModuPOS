using Microsoft.AspNetCore.Mvc;
using ModuPOS.Api.Services;
using ModuPOS.Shared.DTOs.MetodoPago;

namespace ModuPOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetodosPagoController : ControllerBase
    {
        private readonly IMetodosPagoService _service;

        public MetodosPagoController(IMetodosPagoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetodoPagoResponse>>> Get()
        {
            return Ok(await _service.ObtenerMetodosPagoAsync());
        }

        [HttpPost]
        public async Task<ActionResult<MetodoPagoResponse>> Post([FromBody] CrearMetodoPagoRequest request)
        {
            var resultado = await _service.CrearMetodoPagoAsync(request);
            return CreatedAtAction(nameof(Get), new { id = resultado.Id }, resultado);
        }
    }
}
