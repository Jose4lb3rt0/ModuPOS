using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModuPOS.Api.Entities.Identity;
using ModuPOS.Api.Settings;
using ModuPOS.Shared.Constants;
using ModuPOS.Shared.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ModuPOS.Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UsuarioAplicacion> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwt;

        public AuthService(
        UserManager<UsuarioAplicacion> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwtOptions.Value;
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var usuario = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new InvalidOperationException("Credenciales inválidas.");

            if (!usuario.EstaActivo) throw new InvalidOperationException("Esta cuenta está desactivada. Contacta al administrador.");

            if (!await _userManager.CheckPasswordAsync(usuario, request.Password))
                throw new InvalidOperationException("Credenciales inválidas.");

            var roles = await _userManager.GetRolesAsync(usuario);
            var rol = roles.FirstOrDefault() ?? Roles.Cajero;

            return GenerarToken(usuario, rol);
        }

        public async Task<AuthResponse> RegistrarAsync(RegisterRequest request)
        {
            if (request.Rol != Roles.Administrador && request.Rol != Roles.Cajero)
                throw new InvalidOperationException($"Rol '{request.Rol}' no válido. Usa '{Roles.Administrador}' o '{Roles.Cajero}'.");

            var existente = await _userManager.FindByEmailAsync(request.Email);

            if (existente is not null)
                throw new InvalidOperationException($"Ya existe un usuario con el email '{request.Email}'.");

            var usuario = new UsuarioAplicacion
            {
                UserName = request.Email,
                Nombres = request.Nombres,
                ApellidoPaterno = request.ApellidoPaterno,
                ApellidoMaterno = request.ApellidoMaterno,
                Email = request.Email,
                EstaActivo = true
            };

            var resultado = await _userManager.CreateAsync(usuario, request.Password);

            if (!resultado.Succeeded)
            {
                var errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error al crear usuario: {errores}");
            }

            if (!await _roleManager.RoleExistsAsync(request.Rol))
                await _roleManager.CreateAsync(new IdentityRole(request.Rol));

            await _userManager.AddToRoleAsync(usuario, request.Rol);

            return GenerarToken(usuario, request.Rol);
        }

        public async Task<bool> DesactivarUsuarioAsync(string userId)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario is null) return false;

            usuario.EstaActivo = false;
            await _userManager.UpdateAsync(usuario);
            return true;
        }

        private AuthResponse GenerarToken(UsuarioAplicacion usuario, string rol)
        {
            var expiracion = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id),
                new(ClaimTypes.Email, usuario.Email!),
                new(ClaimTypes.Name, usuario.Nombres),
                new(ClaimTypes.Surname, usuario.ApellidoPaterno + " " + usuario.ApellidoMaterno),
                new(ClaimTypes.Role, rol),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expiracion,
                signingCredentials: credenciales
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new AuthResponse(
                Token: tokenString,
                Expiracion: expiracion,
                UserId: usuario.Id,
                Nombres: usuario.Nombres,
                ApellidoPaterno: usuario.ApellidoPaterno,
                ApellidoMaterno: usuario.ApellidoMaterno,
                Email: usuario.Email!,
                Rol: rol
            );
        }
    }
}
