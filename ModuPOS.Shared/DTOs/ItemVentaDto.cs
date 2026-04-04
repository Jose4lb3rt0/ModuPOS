using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs
{
    public class ItemVentaDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        // precio lo lee de la BD para evitar fraudes
    }
}
