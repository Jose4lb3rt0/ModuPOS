using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Services
{
    public interface VentasService
    {
        Task<VentaResult> RegistrarVentaAsync(RegistrarVentaRequest request);
        Task<VentaResponse?> ObtenerVentaAsync(int id);
    }
}
