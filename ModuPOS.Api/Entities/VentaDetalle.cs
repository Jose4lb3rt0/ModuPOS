namespace ModuPOS.Api.Entities
{
    public class VentaDetalle
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public Venta Venta { get; set; } = null!;

        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public int Cantidad { get; set; }
        public decimal PrecioUnitarioHistorico { get; set; }

        public decimal SubtotalLinea { get; set; }  // Columna calculada *
    }
}
