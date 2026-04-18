using Microsoft.AspNetCore.Mvc;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Producto;

namespace ModuPOS.Api.Services
{
    public interface IProductosService
    {
        Task<PagedResponse<ProductoResponse>> ObtenerProductosAsync(int pageIndex, int pageSize);
        Task<PagedResponse<ProductoResponse>> BuscarProductosAsync(BuscarProductosRequest request);
        Task<ProductoResponse?> ObtenerProductoPorIdAsync(int id);
        Task<ProductoResponse> CrearProductoAsync(CrearProductoRequest request, IFormFile? archivoImagen, IImagenService imagenService);
        Task<ProductoResponse?> ActualizarProductoAsync(ActualizarProductoRequest request, IFormFile? archivoImagen, IImagenService imagenService);
        Task<bool> EliminarProductoAsync(int id);
        Task<ProductoResponse> AjustarStockAsync(AjusteStockRequest request);
    }
}
