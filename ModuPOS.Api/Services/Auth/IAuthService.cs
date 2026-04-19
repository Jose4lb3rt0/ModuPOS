using ModuPOS.Shared.DTOs.Auth;

namespace ModuPOS.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegistrarAsync(RegisterRequest request);
        Task<bool> DesactivarUsuarioAsync(string userId);
    }
}
