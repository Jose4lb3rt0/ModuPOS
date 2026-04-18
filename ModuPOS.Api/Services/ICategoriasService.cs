using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Categoria;

namespace ModuPOS.Api.Services
{
    public interface ICategoriasService
    {
        Task<PagedResponse<CategoriaResponse>> ObtenerCategoriasAsync(int pageIndex, int pageSize);
        Task<CategoriaResponse?> ObtenerCategoriaAsync(int id);
        Task<CategoriaResponse> CrearCategoriaAsync(CrearCategoriaRequest req, IFormFile? archivoImagen, IImagenService imagenService);
        Task<CategoriaResponse?> ActualizarCategoriaAsync(ActualizarCategoriaRequest req, IFormFile? archivoImagen, IImagenService imagenService);
        Task<bool> EliminarCategoriaAsync(int id);
    }
}
