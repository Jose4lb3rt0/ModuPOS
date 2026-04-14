using Microsoft.AspNetCore.Components.Forms;
using ModuPOS.Shared.DTOs.Categoria;

namespace ModuPOS.Client.Services.Categoria
{
    public interface ICategoriaClientService
    {
        Task<List<CategoriaResponse>> ObtenerTodasAsync();
        Task<CategoriaResponse?> ObtenerPorIdAsync(int id);
        Task<CategoriaResponse?> CrearAsync(CrearCategoriaRequest request, IBrowserFile? imagen);
        Task<CategoriaResponse?> ActualizarAsync(ActualizarCategoriaRequest request, IBrowserFile? imagen);
        Task<bool> EliminarAsync(int id);
    }
}
