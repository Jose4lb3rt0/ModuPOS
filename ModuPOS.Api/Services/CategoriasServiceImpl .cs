using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Shared.DTOs;
using ModuPOS.Shared.DTOs.Categoria;
using ModuPOS.Shared.DTOs.Imagen;

namespace ModuPOS.Api.Services
{
    public class CategoriasServiceImpl : ICategoriasService
    {
        private readonly ModuPosDbContext _db;

        public CategoriasServiceImpl(ModuPosDbContext db)
        {
            _db = db;
        }

        //1. CREAR
        public async Task<CategoriaResponse> CrearCategoriaAsync(
            CrearCategoriaRequest req,
            IFormFile? archivoImagen,
            IImagenService imagenService)
        {
            //entradas
            string nombreLimpio = req.Nombre.Trim();
            int? padreId = (req.CategoriaPadreId <= 0) ? null : req.CategoriaPadreId;

            //validar nombre unico en el nivel
            bool existeNombre = await _db.Categorias.AnyAsync(c =>
                c.Nombre == nombreLimpio &&
                c.CategoriaPadreId == padreId &&
                !c.IsDeleted);

            if (existeNombre)
                throw new InvalidOperationException($"Ya existe una categoría llamada '{nombreLimpio}' en este nivel.");

            //validar existencia
            if (padreId.HasValue)
            {
                bool padreExiste = await _db.Categorias.AnyAsync(c => c.Id == padreId.Value && !c.IsDeleted);
                if (!padreExiste)
                    throw new InvalidOperationException($"La categoría padre con Id {padreId} no existe.");
            }

            //imagen
            var imagen = await imagenService.SubirYPersistirAsync(archivoImagen, _db);

            var categoria = new Categoria()
            {
                Nombre = nombreLimpio,
                Descripcion = req.Descripcion.Trim(),
                Color = req.Color,
                PopupInformacion = req.PopupInformacion?.Trim(),
                CategoriaPadreId = padreId,
                ImagenId = imagen?.Id,
                Mayoreo1 = req.Mayoreo1,
                Mayoreo2 = req.Mayoreo2,
                TipoPrecioMayoreo = req.TipoPrecioMayoreo
            };

            _db.Categorias.Add(categoria);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch
            {
                if (imagen is not null) await imagenService.EliminarAsync(imagen.ProveedorId);
                throw;
            }

            return await ObtenerCategoriaAsync(categoria.Id) ?? throw new InvalidOperationException("Error al recuperar la categoría recién creada.");
        }

        //2. ACTUALIZAR
        public async Task<CategoriaResponse?> ActualizarCategoriaAsync(
           ActualizarCategoriaRequest req,
           IFormFile? archivoImagen,
           IImagenService imagenService)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Imagen)
                .FirstOrDefaultAsync(c => c.Id == req.Id);

            if (categoria is null) return null;

            var padreId = req.CategoriaPadreId <= 0 ? null : req.CategoriaPadreId;

            //controlar jerarquia
            if (padreId.HasValue)
            {
                //una categoría no puede ser su propio padre
                if (padreId.Value == req.Id)
                    throw new InvalidOperationException(
                        "Una categoría no puede ser su propio padre.");

                //y tampoco puede moverse a una subcategoría propia
                if (await EsDescendienteAsync(req.Id, padreId.Value))
                    throw new InvalidOperationException(
                        "No se puede mover una categoría a una de sus propias subcategorías.");

                await ValidarPadreExisteAsync(padreId);
            }

            if (req.Nombre is not null && req.Nombre.Trim() != categoria.Nombre)
                await ValidarNombreUnicoAsync(
                    req.Nombre.Trim(),
                    padreId ?? categoria.CategoriaPadreId,
                    excluirId: req.Id);

            categoria.Nombre = req.Nombre?.Trim() ?? categoria.Nombre;
            categoria.Descripcion = req.Descripcion?.Trim() ?? categoria.Descripcion;
            categoria.Color = req.Color ?? categoria.Color;
            categoria.PopupInformacion = req.PopupInformacion?.Trim() ?? categoria.PopupInformacion;
            categoria.CategoriaPadreId = padreId ?? categoria.CategoriaPadreId;
            categoria.Mayoreo1 = req.Mayoreo1 ?? categoria.Mayoreo1;
            categoria.Mayoreo2 = req.Mayoreo2 ?? categoria.Mayoreo2;
            categoria.TipoPrecioMayoreo = req.TipoPrecioMayoreo ?? categoria.TipoPrecioMayoreo;

            if (req.QuitarImagen || archivoImagen is { Length: > 0 })
            {
                if (categoria.Imagen is not null)
                {
                    await imagenService.EliminarAsync(categoria.Imagen.ProveedorId);
                    _db.Imagenes.Remove(categoria.Imagen);
                    categoria.ImagenId = null;
                }
            }

            if (archivoImagen is { Length: > 0 })
            {
                var nueva = await imagenService.SubirYPersistirAsync(archivoImagen, _db);
                if (nueva is not null)
                    categoria.ImagenId = nueva.Id;
            }

            await _db.SaveChangesAsync();
            return await ProyectarAsync(categoria.Id);
        }

        //3. OBTENER TODAS CON PAGINACION
        public async Task<PagedResponse<CategoriaResponse>> ObtenerCategoriasAsync(int pageIndex, int pageSize)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            pageIndex = Math.Max(pageIndex, 0);

            var total = await _db.Categorias.CountAsync();

            //proyección directa a tipo anónimo, EF genera sql plano
            var raw = await _db.Categorias
                .OrderBy(c => c.Nombre)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Descripcion,
                    c.Color,
                    c.PopupInformacion,
                    c.Mayoreo1,
                    c.Mayoreo2,
                    c.TipoPrecioMayoreo,
                    c.CategoriaPadreId,
                    CategoriaPadreNombre = c.CategoriaPadre != null
                        ? c.CategoriaPadre.Nombre : null,
                    TotalProductos = c.Productos.Count(p => !p.IsDeleted),
                    Imagen = c.Imagen == null ? (object?)null : new
                    {
                        c.Imagen.Id,
                        c.Imagen.Url,
                        c.Imagen.Nombre
                    },
                    Subcategorias = c.Subcategorias
                        .Where(s => !s.IsDeleted)
                        .Select(s => new { s.Id, s.Nombre, s.Color })
                        .ToList()
                })
                .ToListAsync();

            var items = raw.Select(c => new CategoriaResponse(
                Id: c.Id,
                Nombre: c.Nombre,
                Descripcion: c.Descripcion ?? string.Empty,
                Color: c.Color,
                PopupInformacion: c.PopupInformacion,
                Mayoreo1: c.Mayoreo1,
                Mayoreo2: c.Mayoreo2,
                TipoPrecioMayoreo: c.TipoPrecioMayoreo,
                CategoriaPadreId: c.CategoriaPadreId,
                CategoriaPadreNombre: c.CategoriaPadreNombre,
                TotalProductos: c.TotalProductos,
                Imagen: c.Imagen is null ? null
                                        : new ImagenResponse(
                                            ((dynamic)c.Imagen).Id,
                                            ((dynamic)c.Imagen).Url,
                                            ((dynamic)c.Imagen).Nombre),
                Subcategorias: c.Subcategorias
                    .Select(s => new CategoriaResumenResponse(s.Id, s.Nombre, s.Color))
                    .ToList()
            )).ToList();

            return PagedResponse<CategoriaResponse>.Montar(items, total, pageIndex, pageSize);
        }

        //4. OBTENER POR ID
        public async Task<CategoriaResponse?> ObtenerCategoriaAsync(int id) => 
            await ProyectarAsync(id);

        //5. ELIMINAR
        public async Task<bool> EliminarCategoriaAsync(int id)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Subcategorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria is null) return false;

            if (categoria.Subcategorias.Any())
                throw new InvalidOperationException(
                    $"No se puede eliminar '{categoria.Nombre}' porque tiene " +
                    $"{categoria.Subcategorias.Count} subcategoría(s). " +
                    $"Elimínalas o reasígnalas primero.");

            categoria.IsDeleted = true;
            categoria.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        //HELPER PROYECCIÓN DETALLADA POR ID
        private async Task<CategoriaResponse?> ProyectarAsync(int id)
        {
            var c = await _db.Categorias
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Nombre,
                    x.Descripcion,
                    x.Color,
                    x.PopupInformacion,
                    x.Mayoreo1,
                    x.Mayoreo2,
                    x.TipoPrecioMayoreo,
                    x.CategoriaPadreId,
                    CategoriaPadreNombre = x.CategoriaPadre != null
                        ? x.CategoriaPadre.Nombre : null,
                    TotalProductos = x.Productos.Count(p => !p.IsDeleted),
                    Imagen = x.Imagen == null ? null : new
                    {
                        x.Imagen.Id,
                        x.Imagen.Url,
                        x.Imagen.Nombre
                    },
                    Subcategorias = x.Subcategorias
                        .Where(s => !s.IsDeleted)
                        .Select(s => new { s.Id, s.Nombre, s.Color })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (c is null) return null;

            return new CategoriaResponse(
                Id: c.Id,
                Nombre: c.Nombre,
                Descripcion: c.Descripcion ?? string.Empty,
                Color: c.Color,
                PopupInformacion: c.PopupInformacion,
                Mayoreo1: c.Mayoreo1,
                Mayoreo2: c.Mayoreo2,
                TipoPrecioMayoreo: c.TipoPrecioMayoreo,
                CategoriaPadreId: c.CategoriaPadreId,
                CategoriaPadreNombre: c.CategoriaPadreNombre,
                TotalProductos: c.TotalProductos,
                Imagen: c.Imagen is null ? null
                                        : new ImagenResponse(
                                            c.Imagen.Id,
                                            c.Imagen.Url,
                                            c.Imagen.Nombre),
                Subcategorias: c.Subcategorias
                    .Select(s => new CategoriaResumenResponse(s.Id, s.Nombre, s.Color))
                    .ToList()
            );
        }

        //HELPER JERARQUÍA
        private async Task<bool> EsDescendienteAsync(int ancestroId, int posibleDescendienteId)
        {
            var idActual = (int?)posibleDescendienteId;

            while (idActual.HasValue)
            {
                if (idActual.Value == ancestroId) return true;

                idActual = await _db.Categorias
                    .Where(c => c.Id == idActual.Value)
                    .Select(c => c.CategoriaPadreId)
                    .FirstOrDefaultAsync();
            }

            return false;
        }

        //HELPER VALIDA NOMBRE
        private async Task ValidarNombreUnicoAsync(
            string nombre, int? padreId, int? excluirId)
        {
            bool duplicado = await _db.Categorias.AnyAsync(c =>
                c.Nombre == nombre &&
                c.CategoriaPadreId == padreId &&
                (excluirId == null || c.Id != excluirId.Value));

            if (duplicado)
                throw new InvalidOperationException(
                    $"Ya existe una categoría '{nombre}' en ese nivel.");
        }

        //HELPER VALIDA EXISTENCIA PADRE
        private async Task ValidarPadreExisteAsync(int? padreId)
        {
            if (!padreId.HasValue) return;

            bool existe = await _db.Categorias
                .AnyAsync(c => c.Id == padreId.Value);

            if (!existe)
                throw new InvalidOperationException(
                    $"No existe la categoría padre con Id {padreId}.");
        }
    }
}
