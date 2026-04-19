using System.Net.Http.Headers;

namespace ModuPOS.Client.Auth
{
    public class AuthDelegatingHandler : DelegatingHandler
    {
        private readonly TokenService _tokenService;

        public AuthDelegatingHandler(TokenService tokenService)
            => _tokenService = tokenService;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _tokenService.ObtenerAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
