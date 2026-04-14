using Microsoft.AspNetCore.Components.Forms;
using ModuPOS.Shared.DTOs.Producto;

namespace ModuPOS.Client.Services.Producto
{
    public interface IProductoClientService
    {
        Task<List<ProductoResponse>> ObtenerTodosAsync();
        Task<List<ProductoResponse>> BuscarAsync(string termino);
        Task<ProductoResponse?>      CrearAsync(CrearProductoRequest request, IBrowserFile? imagen);
        Task<ProductoResponse?>      ActualizarAsync(ActualizarProductoRequest request, IBrowserFile? imagen);
        Task<bool>                   EliminarAsync(int id);
        Task<ProductoResponse?>      AjustarStockAsync(AjusteStockRequest request);
    }
}
