using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Venta
{
    public class VentaResponse
    {
        public int Id { get; set; }
        public string Folio { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public List<DetalleVentaResponse> Detalles { get; set; } = new();
    }
}
