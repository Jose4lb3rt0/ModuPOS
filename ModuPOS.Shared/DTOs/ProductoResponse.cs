using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs
{
    public class ProductoResponse
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioActual { get; set; }
        public int Stock { get; set; }
    }
}
