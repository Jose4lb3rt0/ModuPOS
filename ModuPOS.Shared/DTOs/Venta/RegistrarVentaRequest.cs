using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Venta
{
    public class RegistrarVentaRequest
    {
        public string Folio { get; set; } = string.Empty;
        public int MetodoPagoId { get; set; }

        public decimal DescuentoTotal { get; set; }

        public List<ItemVentaDto> Items { get; set; } = new();
    }
}
