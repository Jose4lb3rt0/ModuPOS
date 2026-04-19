using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuPOS.Api.Services.Auth;
using ModuPOS.Shared.Constants;
using ModuPOS.Shared.DTOs.Auth;

namespace ModuPOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
            => Ok(await _authService.LoginAsync(request));

        [HttpPost("registrar")]
        [Authorize(Policy = Policies.SoloAdmin)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<AuthResponse>> Registrar([FromBody] RegisterRequest request)
        {
            var resultado = await _authService.RegistrarAsync(request);
            return CreatedAtAction(nameof(Login), resultado);
        }

        [HttpPatch("{userId}/desactivar")]
        [Authorize(Policy = Policies.SoloAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Desactivar(string userId)
        {
            var ok = await _authService.DesactivarUsuarioAsync(userId);
            return ok ? NoContent() : NotFound();
        }
    }
}
