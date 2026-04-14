using ModuPOS.Shared.DTOs.Categoria;

namespace ModuPOS.Api.Services
{
    public interface ICategoriasService
    {
        Task<CategoriaResponse> CrearCategoriaAsync(CrearCategoriaRequest req, IFormFile? archivoImagen, IImagenService imagenService);
        Task<CategoriaResponse?> ActualizarCategoriaAsync(ActualizarCategoriaRequest req, IFormFile? archivoImagen, IImagenService imagenService);
        Task<List<CategoriaResponse>> ObtenerCategoriasAsync();
        Task<CategoriaResponse?> ObtenerCategoriaAsync(int id);
        Task<bool> EliminarCategoriaAsync(int id);
    }
}
