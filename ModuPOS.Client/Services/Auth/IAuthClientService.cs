using ModuPOS.Shared.DTOs.Auth;

namespace ModuPOS.Client.Services.Auth
{
    public interface IAuthClientService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
