namespace ModuPOS.Api.Entities
{
    public class Producto : BaseEntity
    {
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioActual { get; set; }
        public int Stock { get; set; }
    }
}
