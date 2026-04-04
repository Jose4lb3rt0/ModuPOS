namespace ModuPOS.Api.Entities
{
    public class Producto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioActual { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
