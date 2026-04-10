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

        [HttpGet]
        public async Task<ActionResult<List<VentaResponse>>> ObtenerVentas()
        {
            var ventas = await _ventasService.ObtenerTodasLasVentasAsync();
            return Ok(ventas);
        }

        [HttpGet("historial")]
        [ProducesResponseType(typeof(List<VentaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<VentaResponse>>> ObtenerHistorial(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fin)
        {
            if (inicio > fin) return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");

            var ventas = await _ventasService.ObtenerVentasPorRangoAsync(inicio, fin);
            return Ok(ventas);
        }
    }
}
