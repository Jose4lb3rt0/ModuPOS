using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal DescuentoTotal { get; set; }

        public decimal Total => Subtotal + Impuestos - DescuentoTotal;

        public List<DetalleVenta> Items { get; set; } = new();
    }
}
