using ModuPOS.Shared.DTOs.Imagen;
using ModuPOS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Categoria
{
    public class CrearCategoriaRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Color { get; set; } = "#FFFFFF";
        public string? PopupInformacion { get; set; }

        public int? CategoriaPadreId { get; set; }

        //public string? ImagenUrl { get; set; }
        //public string? ImagenNombre { get; set; }

        //precios mayoreo
        public decimal Mayoreo1 { get; set; }
        public decimal Mayoreo2 { get; set; }
        public TipoMayoreo TipoPrecioMayoreo { get; set; } = TipoMayoreo.PrecioFijo;
    }
}
