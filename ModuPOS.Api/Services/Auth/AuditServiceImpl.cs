using System.Security.Claims;

namespace ModuPOS.Api.Services.Auth
{
    public class AuditServiceImpl : IAuditService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        //IHttpContextAccessor permite acceder al HttpContext fuera de un controller
        //el DbContext vive en la capa de datos y no tiene acceso directo a HTTP
        //este servicio actúa como puente
        public AuditServiceImpl(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? ObtenerUsuarioActual()
        {
            //NameIdentifier es el claim estándar que contiene el Id del usuario
            //Lo generamos así en AuthService al crear el token
            return _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
