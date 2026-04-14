using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.MetodoPago
{
    public class MetodoPagoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
