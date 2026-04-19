using Microsoft.AspNetCore.Identity;

namespace ModuPOS.Api.Entities.Identity
{
    public class UsuarioAplicacion : IdentityUser
    {
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;

        public bool EstaActivo { get; set; } = true;
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    }
}
