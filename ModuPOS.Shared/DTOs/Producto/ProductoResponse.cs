using ModuPOS.Shared.DTOs.Categoria;
using ModuPOS.Shared.DTOs.Imagen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Producto
{
    public record ProductoResponse
    (
        int Id,
        string SKU,
        string Nombre,
        decimal PrecioActual,
        int Stock,
        CategoriaResumenResponse? Categoria,
        ImagenResponse? Imagen
    );
}
