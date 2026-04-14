using ModuPOS.Shared.DTOs.Producto;

namespace ModuPOS.Client.Services
{
    public interface IProductoClientService
    {
        Task<List<ProductoResponse>> ObtenerTodosAsync();
        Task<List<ProductoResponse>> BuscarAsync(string termino);
        Task<ProductoResponse?> CrearAsync(CrearProductoRequest request);
        Task<ProductoResponse?> ActualizarAsync(ActualizarProductoRequest request);
        Task<bool> EliminarAsync(int id);
    }
}
