using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Producto
{
    public class CrearProductoRequest
    {
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioActual { get; set; }
        public int Stock { get; set; }
    }
}
