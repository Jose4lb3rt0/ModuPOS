using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;

namespace ModuPOS.Api.Services
{
    public record ImagenSubidaDto(string Url, string PublicId, string NombreArchivo);

    public interface IImagenService
    {
        Task<ImagenSubidaDto> SubirAsync(Stream stream, string nombreArchivo);
        Task<bool> EliminarAsync(string publicId);
        Task<Imagen?> SubirYPersistirAsync(IFormFile? archivo, ModuPosDbContext db);
    }
}
