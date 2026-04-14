using ModuPOS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Categoria
{
    public class ActualizarCategoriaRequest
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Color { get; set; }
        public string? PopupInformacion { get; set; }
        public int? CategoriaPadreId { get; set; }

        public decimal? Mayoreo1 { get; set; }
        public decimal? Mayoreo2 { get; set; }
        public TipoMayoreo? TipoPrecioMayoreo { get; set; }

        //public string? ImagenUrl { get; set; }
        //public string? ImagenNombre { get; set; }
        public bool QuitarImagen { get; set; }
    }
}
