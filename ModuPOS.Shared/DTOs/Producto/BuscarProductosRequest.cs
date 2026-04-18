using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Producto
{
    public class BuscarProductosRequest
    {
        public string? Termino { get; set; }
        public int? CategoriaId { get; set; }
        public int PageIndex { get; set; } = 0; 
        public int PageSize { get; set; } = 20;
    }
}
