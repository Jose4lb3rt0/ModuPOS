using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Categoria;

namespace ModuPOS.Client.Services.Categoria
{
    public interface ICategoriaClientService
    {
        Task<PagedResponse<CategoriaResponse>> ObtenerTodasAsync(int pageIndex = 0, int pageSize = 20);
        Task<CategoriaResponse?> ObtenerPorIdAsync(int id);
        Task<CategoriaResponse?> CrearAsync(CrearCategoriaRequest request, IBrowserFile? imagen);
        Task<CategoriaResponse?> ActualizarAsync(ActualizarCategoriaRequest request, IBrowserFile? imagen);
        Task<bool> EliminarAsync(int id);
    }
}
