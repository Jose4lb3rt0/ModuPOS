using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs
{
    public class ResumenVentasResponse
    {
        public DateTime Fecha { get; set; }
        public decimal TotalVendido { get; set; }
        public int CantidadVentas { get; set; }
        public List<VentasPorMetodoPagoDto> DesglosePorMetodo { get; set; } = new();
    }

    public class VentasPorMetodoPagoDto
    {
        public string MetodoPago { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
    }
}
