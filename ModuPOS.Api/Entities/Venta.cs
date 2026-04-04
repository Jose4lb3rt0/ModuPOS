using ModuPOS.Shared.Enums;

namespace ModuPOS.Api.Entities
{
    public class Venta : BaseEntity
    {
        public string Folio { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal DescuentoTotal { get; set; }

        public decimal Total { get; set; } // columna computed

        public EstadoVenta Estado { get; set; }

        public int MetodoPagoId { get; set; }
        public MetodoPago MetodoPago { get; set; } = null!; // puede ser null en runtime, pero EF lo llenará
        public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}
