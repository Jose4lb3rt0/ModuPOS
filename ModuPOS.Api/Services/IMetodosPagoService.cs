using ModuPOS.Shared.DTOs.MetodoPago;

namespace ModuPOS.Api.Services
{
    public interface IMetodosPagoService
    {
        Task<MetodoPagoResponse> CrearMetodoPagoAsync(CrearMetodoPagoRequest request);
        Task<List<MetodoPagoResponse>> ObtenerMetodosPagoAsync();
    }
}
