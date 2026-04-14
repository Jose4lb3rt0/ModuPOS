using Microsoft.AspNetCore.Components.Forms;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Categoria;
using System.Net;
using System.Net.Http.Json;

namespace ModuPOS.Client.Services.Categoria
{
    public class CategoriaClientService : ICategoriaClientService
    {
        private readonly HttpClient _http;
        private const long MaxImageSize = 5 * 1024 * 1024;

        public CategoriaClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CategoriaResponse>> ObtenerTodasAsync() {
            return await _http.GetFromJsonAsync<List<CategoriaResponse>>("api/categorias") ?? new();
        }

        public async Task<CategoriaResponse?> ObtenerPorIdAsync(int id) =>
            await _http.GetFromJsonAsync<CategoriaResponse>($"api/categorias/{id}");

        public async Task<CategoriaResponse?> CrearAsync(
            CrearCategoriaRequest request, IBrowserFile? imagen)
        {
            using var content = BuildMultipart(request, imagen);
            var response = await _http.PostAsync("api/categorias", content);
            return await LeerRespuestaAsync<CategoriaResponse>(response);
        }

        public async Task<CategoriaResponse?> ActualizarAsync(
            ActualizarCategoriaRequest request, IBrowserFile? imagen)
        {
            using var content = BuildMultipart(request, imagen);
            var response = await _http.PatchAsync("api/categorias", content);
            return await LeerRespuestaAsync<CategoriaResponse>(response);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/categorias/{id}");
            if (response.IsSuccessStatusCode) return true;
            if (response.StatusCode == HttpStatusCode.NotFound) return false;

            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            throw new HttpRequestException(error?.Message ?? "Error al eliminar.");
        }

        private static MultipartFormDataContent BuildMultipart<T>(T dto, IBrowserFile? imagen)
        {
            var content = new MultipartFormDataContent();

            foreach (var prop in typeof(T).GetProperties())
            {
                var valor = prop.GetValue(dto);
                if (valor is not null)
                    content.Add(new StringContent(valor.ToString()!), prop.Name);
            }

            if (imagen is { Size: > 0 })
            {
                var stream = imagen.OpenReadStream(MaxImageSize);
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(imagen.ContentType);
                content.Add(streamContent, "archivoImagen", imagen.Name);
            }
            return content;
        }

        private static async Task<T?> LeerRespuestaAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            throw new HttpRequestException(error?.Message ?? $"Error HTTP {(int)response.StatusCode}");
        }
    }
}