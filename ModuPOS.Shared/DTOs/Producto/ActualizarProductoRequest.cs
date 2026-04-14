namespace ModuPOS.Shared.DTOs.Producto
{
    public class ActualizarProductoRequest
    {
        public int Id { get; set; }
        public string? SKU { get; set; }
        public string? Nombre { get; set; }
        public decimal? PrecioActual { get; set; }
        public int? Stock { get; set; }

        public int? CategoriaId { get; set; }

        public string? ImagenUrl { get; set; }
        public string? ImagenNombre { get; set; } 
        public bool QuitarImagen { get; set; }
    }
}