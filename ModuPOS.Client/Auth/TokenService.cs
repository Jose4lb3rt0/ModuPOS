using Microsoft.JSInterop;

namespace ModuPOS.Client.Auth
{
    public class TokenService
    {
        private readonly IJSRuntime _js;
        private const string Clave = "authToken";

        public TokenService(IJSRuntime js) => _js = js;

        public async Task GuardarAsync(string token)
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", Clave);
            await _js.InvokeVoidAsync("localStorage.setItem", Clave, token);
        }

        public async Task<string?> ObtenerAsync()
        {
            try
            {
                return await _js.InvokeAsync<string?>("localStorage.getItem", Clave);
            }
            catch
            {
                return null;
            }
        }

        public async Task EliminarAsync() => await _js.InvokeVoidAsync("localStorage.removeItem", Clave);
    }
}
