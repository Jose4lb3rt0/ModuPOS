using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Services
{
    public interface IVentasService
    {
        Task<VentaResult> RegistrarVentaAsync(RegistrarVentaRequest request);
        Task<VentaResponse?> ObtenerVentaAsync(int id);
    }
}
