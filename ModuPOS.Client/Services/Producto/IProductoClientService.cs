using Microsoft.AspNetCore.Components.Forms;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Producto;

namespace ModuPOS.Client.Services.Producto
{
    public interface IProductoClientService
    {
        Task<PagedResponse<ProductoResponse>> ObtenerTodosAsync(int pageIndex = 0, int pageSize = 20);
        Task<PagedResponse<ProductoResponse>> BuscarAsync(string? termino, int? categoriaId = null, int pageIndex = 0, int pageSize = 20);
        Task<ProductoResponse?>      CrearAsync(CrearProductoRequest request, IBrowserFile? imagen);
        Task<ProductoResponse?>      ActualizarAsync(ActualizarProductoRequest request, IBrowserFile? imagen);
        Task<bool>                   EliminarAsync(int id);
        Task<ProductoResponse?>      AjustarStockAsync(AjusteStockRequest request);
    }
}
