using ModuPOS.Shared.DTOs.Imagen;
using ModuPOS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Categoria
{
    public class CategoriaResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Color { get; set; } = "#FFFFFF";
        public string? PopupInformacion { get; set; }

        public decimal Mayoreo1 { get; set; }
        public decimal Mayoreo2 { get; set; }
        public TipoMayoreo TipoPrecioMayoreo { get; set; }

        public int? CategoriaPadreId { get; set; }
        public string? CategoriaPadreNombre { get; set; }

        public ImagenResponse? Imagen { get; set; }
        public int TotalProductos { get; set; }

        public List<CategoriaResumenResponse> Subcategorias { get; set; } = new();
    }

    public class CategoriaResumenResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Color { get; set; } = "#FFFFFF";
    }
}
