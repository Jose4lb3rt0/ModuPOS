using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Services
{
    public interface IMetodosPagoService
    {
        Task<MetodoPagoResponse> CrearMetodoPagoAsync(CrearMetodoPagoRequest request);
        Task<List<MetodoPagoResponse>> ObtenerMetodosPagoAsync();
    }
}
