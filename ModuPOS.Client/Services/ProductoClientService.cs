using ModuPOS.Shared.DTOs;
using System.Net;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ModuPOS.Client.Services
{
    public class ProductoClientService : IProductoClientService
    {
        private HttpClient _httpClient;

        public ProductoClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //PATCH actualizar
        public async Task<ProductoResponse?> ActualizarAsync(ActualizarProductoRequest request)
        {
            var response = await _httpClient.PatchAsJsonAsync("api/productos", request);
            return await LeerRespuestaAsync<ProductoResponse>(response);
        }

        //GET busqueda
        public async Task<List<ProductoResponse>> BuscarAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return await ObtenerTodosAsync();

            var url = $"api/productos/buscar?termino={Uri.EscapeDataString(termino)}";

            return await _httpClient.GetFromJsonAsync<List<ProductoResponse>>(url)
                   ?? new List<ProductoResponse>();
        }

        //POST crear
        public async Task<ProductoResponse?> CrearAsync(CrearProductoRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/productos", request);
            return await LeerRespuestaAsync<ProductoResponse>(response);
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
            return await _httpClient.GetFromJsonAsync<List<ProductoResponse>>(
                       "api/productos")
                   ?? new List<ProductoResponse>();
        }

        private static async Task<T?> LeerRespuestaAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<T>();

            var error = await response.Content.ReadFromJsonAsync<ErrorDetails>();
            throw new HttpRequestException(error?.Message ?? $"Error HTTP {(int)response.StatusCode}");
        }
    }
}
