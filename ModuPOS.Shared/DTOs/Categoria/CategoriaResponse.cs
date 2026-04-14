using ModuPOS.Shared.DTOs.Imagen;
using ModuPOS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs.Categoria
{
    public record CategoriaResponse
    (
        int Id,
        string Nombre,
        string Descripcion,
        string Color,
        string? PopupInformacion,

        decimal Mayoreo1,
        decimal Mayoreo2,
        TipoMayoreo TipoPrecioMayoreo,

        int? CategoriaPadreId,
        string? CategoriaPadreNombre,

        int TotalProductos,
        ImagenResponse? Imagen,

        List<CategoriaResumenResponse> Subcategorias
    );

    public record CategoriaResumenResponse(int Id, string Nombre, string Color);
}
