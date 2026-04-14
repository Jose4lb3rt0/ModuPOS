using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Venta
{
    public class DetalleVentaResponse
    {
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitarioHistorico { get; set; }
        public decimal SubtotalLinea { get; set; }
    }
}
