using Microsoft.AspNetCore.Components.Forms;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Producto;
using System.Net;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ModuPOS.Client.Services.Producto
{
    public class ProductoClientService : IProductoClientService
    {
        private readonly HttpClient _httpClient;
        private const long MaxImageSize = 5 * 1024 * 1024; //5 mb

        public ProductoClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //POST crear
        public async Task<ProductoResponse?> CrearAsync(CrearProductoRequest request, IBrowserFile? imagen)
        {
            using var content = BuildMultipart(request, imagen); //el cliente manda MultipartFormDataContent
            var response = await _httpClient.PostAsync("api/productos", content); //el api recibe con FromForm
            return await LeerRespuestaAsync<ProductoResponse>(response);
        }

        //PATCH actualizar
        public async Task<ProductoResponse?> ActualizarAsync(ActualizarProductoRequest request, IBrowserFile? imagen)
        {
            using var content = BuildMultipart(request, imagen);
            var response = await _httpClient.PatchAsync("api/productos", content);
            return await LeerRespuestaAsync<ProductoResponse>(response);
        }

        //GET busqueda
        public async Task<List<ProductoResponse>> BuscarAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino)) return await ObtenerTodosAsync();

            var url = $"api/productos/buscar?termino={Uri.EscapeDataString(termino)}";

            return await _httpClient.GetFromJsonAsync<List<ProductoResponse>>(url)
                   ?? new List<ProductoResponse>();
        }

        //DELETE 
        public async Task<bool> EliminarAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/productos/{id}");

            if (response.IsSuccessStatusCode) return true;

            if (response.StatusCode == HttpStatusCode.NotFound) return false;

            //deserializar ErrorDetails para obtener
            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();

            throw new HttpRequestException(error?.Message ?? "Error desconocido al eliminar el producto.");
        }

        //GET todos
        public async Task<List<ProductoResponse>> ObtenerTodosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ProductoResponse>>("api/productos") 
                ?? new List<ProductoResponse>();
        }

        //PATCH ajustar stock
        public async Task<ProductoResponse?> AjustarStockAsync(AjusteStockRequest request)
        {
            var response = await _httpClient.PatchAsJsonAsync("api/productos/ajustar-stock", request);
            return await LeerRespuestaAsync<ProductoResponse>(response);
        }

        private static async Task<T?> LeerRespuestaAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<T>();

            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            throw new HttpRequestException(error?.Message ?? $"Error HTTP {(int)response.StatusCode}");
        }

        private static MultipartFormDataContent BuildMultipart<T>(T dto, IBrowserFile? imagen)
        {
            var content = new MultipartFormDataContent();

            //serializar cada propiedad del DTO como campo de formulario
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                var valor = prop.GetValue(dto);
                if (valor is not null)
                    content.Add(new StringContent(valor.ToString()!), prop.Name);
            }

            //si el archivo se proporcionó y no supera el límite
            if (imagen is { Size: > 0 })
            {
                if (imagen.Size > MaxImageSize)
                    throw new InvalidOperationException(
                        $"La imagen no puede superar 5 MB. Tamaño actual: {imagen.Size / 1024 / 1024:F1} MB.");

                var stream = imagen.OpenReadStream(MaxImageSize);

                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(imagen.ContentType);

                content.Add(streamContent, "archivoImagen", imagen.Name);
            }

            return content;
        }
    }
}
