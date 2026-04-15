using Microsoft.AspNetCore.Mvc;
using ModuPOS.Shared.DTOs.Producto;

namespace ModuPOS.Api.Services
{
    public interface IProductosService
    {
        Task<ProductoResponse> CrearProductoAsync(CrearProductoRequest request, IFormFile? archivoImagen, IImagenService imagenService);
        Task<ProductoResponse?> ActualizarProductoAsync(ActualizarProductoRequest request, IFormFile? archivoImagen, IImagenService imagenService);
        Task<List<ProductoResponse>> ObtenerProductosAsync();
        Task<bool> EliminarProductoAsync(int id);
        Task<List<ProductoResponse>> BuscarProductosAsync(string termino);
        Task<ProductoResponse?> ObtenerProductoPorIdAsync(int id);
        Task<ProductoResponse> AjustarStockAsync(AjusteStockRequest request);
    }
}
