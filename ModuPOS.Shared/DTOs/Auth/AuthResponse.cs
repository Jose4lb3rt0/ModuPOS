using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Auth
{
    public record AuthResponse(
        string Token,
        DateTime Expiracion,
        string UserId,
        string Nombres,
        string ApellidoPaterno,
        string ApellidoMaterno,
        string Email,
        string Rol
    );
}
