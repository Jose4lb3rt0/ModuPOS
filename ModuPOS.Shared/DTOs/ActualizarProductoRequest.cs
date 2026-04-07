namespace ModuPOS.Shared.DTOs
{
    public class ActualizarProductoRequest
    {
        public int Id { get; set; }
        public string? SKU { get; set; }
        public string? Nombre { get; set; }
        public decimal? PrecioActual { get; set; }
        public int? Stock { get; set; }
    }
}