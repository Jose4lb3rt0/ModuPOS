using ModuPOS.Shared.DTOs.Categoria;
using ModuPOS.Shared.Enums;

namespace ModuPOS.Api.Entities
{
    public class Categoria : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Color { get; set; } = "#FFFFFF";
        public string? PopupInformacion { get; set; }

        public decimal Mayoreo1 { get; set; }
        public decimal Mayoreo2 { get; set; }
        public TipoMayoreo TipoPrecioMayoreo { get; set; } = TipoMayoreo.PrecioFijo;

        //imagen es opcional
        public int? ImagenId { get; set; }
        public virtual Imagen? Imagen { get; set; }

        //una categoria puede tener muchas subcategorias
        public int? CategoriaPadreId { get; set; }
        public virtual Categoria? CategoriaPadre { get; set; }
        public virtual ICollection<Categoria> Subcategorias { get; set; } = new List<Categoria>();

        //relacion con los productos
        public virtual ICollection<Producto> Productos      { get; set; } = new List<Producto>();
    }
}

