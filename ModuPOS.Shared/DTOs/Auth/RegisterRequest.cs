using ModuPOS.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Auth
{
    public class RegisterRequest
    {
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string Rol { get; set; } = Roles.Cajero;
    }
}
