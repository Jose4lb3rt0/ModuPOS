namespace ModuPOS.Api.Entities
{
    public class Producto : BaseEntity
    {
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioActual { get; set; }
        public int Stock { get; set; }

        public int? CategoriaId { get; set; }
        public virtual Categoria? Categoria { get; set; }

        public int? ImagenId { get; set; }
        public virtual Imagen? Imagen { get; set; }
    }
}
