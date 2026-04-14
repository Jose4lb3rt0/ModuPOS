using ModuPOS.Shared.DTOs.Categoria;

namespace ModuPOS.Api.Services
{
    public interface ICategoriasService
    {
        Task<List<CategoriaResponse>> ObtenerCategoriasAsync();
        Task<CategoriaResponse?> ObtenerCategoriaAsync(int id);
        Task<CategoriaResponse> CrearCategoriaAsync(CrearCategoriaRequest req);
        Task<CategoriaResponse?> ActualizarCategoriaAsync(ActualizarCategoriaRequest req);
        Task<bool> EliminarCategoriaAsync(int id);
    }
}
