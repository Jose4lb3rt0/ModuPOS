using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Api.Services;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.Enums;

namespace ModuPOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentasService _ventasService;

        public VentasController(IVentasService ventasService)
        {
            _ventasService = ventasService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(VentaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VentaResponse>> RegistrarVenta(
            [FromBody] RegistrarVentaRequest request)
        {
            var resultado = await _ventasService.RegistrarVentaAsync(request);

            if (!resultado.EsExitoso) return BadRequest(resultado.MensajeError);

            return CreatedAtAction(
                nameof(ObtenerVenta),
                new { id = resultado.Datos!.Id },
                resultado.Datos);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(VentaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VentaResponse>> ObtenerVenta(int id)
        {
            var venta = await _ventasService.ObtenerVentaAsync(id);

            return venta is null
                ? NotFound()
                : Ok(venta);
        }
    }
}
