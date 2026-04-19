using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;

namespace ModuPOS.Client.Auth
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _http;
        private readonly TokenService _tokenService;

        //estado anonimo reutilizable
        private static readonly AuthenticationState Anonimo =
            new(new ClaimsPrincipal(new ClaimsIdentity()));


        public JwtAuthStateProvider(
            HttpClient http,
            TokenService tokenService)
        {
            _http = http;
            _tokenService = tokenService;
        }

        //blazor llama durante prerendering, devolviendo anonimo momentaneamente
        //en lo que js no se encuentra disponible aún
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(Anonimo);

        //llamado desde App.razor al montar el componente raíz
        //cuando el circuito JS ya está activo
        public async Task InicializarAsync() 
        {
            var token = await _tokenService.ObtenerAsync();

            if (string.IsNullOrWhiteSpace(token) || TokenExpirado(token))
            {
                await _tokenService.EliminarAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
                return;
            }

            AplicarToken(token);
            var estado = ConstruirEstado(token);
            NotifyAuthenticationStateChanged(Task.FromResult(estado));
        }

        //login
        public async Task IniciarSesionAsync(string token)
        {
            await _tokenService.GuardarAsync(token);
            AplicarToken(token);
            var estado = ConstruirEstado(token);
            NotifyAuthenticationStateChanged(Task.FromResult(estado));
        }

        //logout
        public async Task CerrarSesionAsync(string token)
        {
            await _tokenService.EliminarAsync();
            _http.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }

        private void AplicarToken(string token)
            => _http.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

        private static AuthenticationState ConstruirEstado(string token)
        {
            var claims = ParsearClaims(token);
            var identidad = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identidad));
        }

        private static IEnumerable<Claim>? ParsearClaims(string token)
            => new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;

        private static bool TokenExpirado(string token)
            => new JwtSecurityTokenHandler().ReadJwtToken(token).ValidTo < DateTime.UtcNow;
    }
}
