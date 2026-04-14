using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using ModuPOS.Api.Settings;

namespace ModuPOS.Api.Services
{
    public class ImagenServiceImpl : IImagenService
    {
        private readonly Cloudinary _cloudinary;

        public ImagenServiceImpl(IOptions<CloudinarySettings> options)
        {
            var cfg = options.Value;
            var cuenta = new Account(cfg.CloudName, cfg.ApiKey, cfg.ApiSecret);
            _cloudinary = new Cloudinary(cuenta) { Api = { Secure = true } };
        }

        public async Task<ImagenSubidaDto> SubirAsync(Stream stream, string nombreArchivo)
        {
            var nombre = Path.GetFileNameWithoutExtension(nombreArchivo);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(nombreArchivo, stream),
                DisplayName = nombre,
                Transformation = new Transformation()
                    .Height(600).Width(600)
                    .Crop("fill")
                    .Gravity("auto")
                    .Quality("auto")
                    .FetchFormat("auto") //webp automatico
            };

            var resultado = await _cloudinary.UploadAsync(uploadParams);

            if (resultado.Error is not null) throw new InvalidOperationException($"Error al subir imagen: {resultado.Error.Message}");

            return new ImagenSubidaDto(
                Url: resultado.SecureUrl.ToString(),
                PublicId: resultado.PublicId,
                NombreArchivo: nombreArchivo
            );
        }

        public async Task<bool> EliminarAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var resultado = await _cloudinary.DestroyAsync(deletionParams);
            return resultado.Result == "ok";
        }
    }
}
