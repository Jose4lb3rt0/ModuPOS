using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Imagen
{
    public class ImagenResponse
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}
