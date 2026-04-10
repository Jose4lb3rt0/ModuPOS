using Microsoft.AspNetCore.Mvc;
using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Services
{
    public interface IProductosService
    {
        Task<ProductoResponse> CrearProductoAsync(CrearProductoRequest request);
        Task<List<ProductoResponse>> ObtenerProductosAsync();
        Task<ProductoResponse?> ActualizarProductoAsync(ActualizarProductoRequest request);
        Task<bool> EliminarProductoAsync(int id);
    }
}
