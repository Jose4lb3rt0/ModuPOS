using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Auth;
using System.Net.Http.Json;

namespace ModuPOS.Client.Services.Auth
{
    public class AuthClientService : IAuthClientService
    {
        private readonly HttpClient _http;  //login usa el HttpClient sin el DelegatingHandler

        public AuthClientService(HttpClient http) => _http = http;

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<AuthResponse>()
                       ?? throw new HttpRequestException("Respuesta vacía del servidor.");

            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            throw new HttpRequestException(error?.Message ?? "Error al iniciar sesión.");
        }
    }
}
