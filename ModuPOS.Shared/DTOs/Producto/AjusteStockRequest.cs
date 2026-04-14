using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Producto
{
    public class AjusteStockRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }
}
