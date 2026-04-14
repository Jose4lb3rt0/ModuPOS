using Azure.Core;
using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
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

        public async Task<CategoriaResponse?> ActualizarCategoriaAsync(ActualizarCategoriaRequest req)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Imagen)
                .FirstOrDefaultAsync(c => c.Id == req.Id);

            if (categoria is null) return null;

            //validar duplicado
            if (req.Nombre is not null && req.Nombre != categoria.Nombre)
            {
                if (await _db.Categorias.AnyAsync(c =>
                    c.Nombre == req.Nombre &&
                    c.CategoriaPadreId == (req.CategoriaPadreId ?? categoria.CategoriaPadreId) &&
                    c.Id != req.Id))
                { 
                    throw new InvalidOperationException($"Ya existe una categoría llamada '{req.Nombre}' en ese nivel.");
                }
            }

            categoria.Nombre = req.Nombre?.Trim() ?? categoria.Nombre;
            categoria.Descripcion = req.Descripcion?.Trim() ?? categoria.Descripcion;
            categoria.Color = req.Color ?? categoria.Color;
            categoria.PopupInformacion = req.PopupInformacion?.Trim() ?? categoria.PopupInformacion;
            categoria.CategoriaPadreId = req.CategoriaPadreId ?? categoria.CategoriaPadreId;
            categoria.Mayoreo1 = req.Mayoreo1 ?? categoria.Mayoreo1;
            categoria.Mayoreo2 = req.Mayoreo2 ?? categoria.Mayoreo2;
            categoria.TipoPrecioMayoreo = req.TipoPrecioMayoreo ?? categoria.TipoPrecioMayoreo;

            if (req.QuitarImagen)
            {
                categoria.ImagenId = null;
            }
            else if (!string.IsNullOrWhiteSpace(req.ImagenUrl))
            {
                var nuevaImagen = new Imagen()
                {
                    Url = req.ImagenUrl!,
                    Nombre = req.ImagenUrl,
                    ProveedorId = req.ImagenUrl!,
                    Proveedor = "Cloudinary"
                };

                _db.Imagenes.Add(nuevaImagen);
                await _db.SaveChangesAsync();

                categoria.ImagenId = nuevaImagen.Id;
            }

            await _db.SaveChangesAsync();
            return await ObtenerCategoriaAsync(categoria.Id);
        }

        public async Task<CategoriaResponse> CrearCategoriaAsync(CrearCategoriaRequest req)
        {
            //verificar duplicado
            if (await _db.Categorias.AnyAsync(c => c.Nombre == req.Nombre && c.CategoriaPadreId == req.CategoriaPadreId)) 
            {
                throw new InvalidOperationException(
                    $"Ya existe una categoría llamada '{req.Nombre}' " +
                    $"en el mismo nivel.");
            }

            //verificar padre existente
            if (req.CategoriaPadreId.HasValue)
            {
                if (await _db.Categorias.AnyAsync(c => c.Id == req.CategoriaPadreId.Value) == false)
                {
                    throw new InvalidOperationException(
                        $"No existe la categoría con id '{req.CategoriaPadreId.Value}'.");
                }
            }

            //imagen proporcionada
            Imagen? imagen = null;
            if (!string.IsNullOrWhiteSpace(req.ImagenUrl))
            {
                imagen = new Imagen()
                {
                    Url = req.ImagenUrl!,
                    Nombre = req.ImagenNombre ?? string.Empty,
                    ProveedorId = req.ImagenUrl!, //la url actúa como id en cloudinary
                    Proveedor = "Cloudinary"
                };

                _db.Imagenes.Add(imagen);
                await _db.SaveChangesAsync();
            }

            var categoria = new Categoria()
            {
                Nombre = req.Nombre.Trim(),
                Descripcion = req.Descripcion.Trim(),
                Color = req.Color,
                PopupInformacion = req.PopupInformacion?.Trim(),
                CategoriaPadreId = req.CategoriaPadreId,
                ImagenId = imagen?.Id,
                Mayoreo1 = req.Mayoreo1,
                Mayoreo2 = req.Mayoreo2,
                TipoPrecioMayoreo = req.TipoPrecioMayoreo
            };

            _db.Categorias.Add(categoria);
            await _db.SaveChangesAsync();

            return await ObtenerCategoriaAsync(categoria.Id) ?? throw new InvalidOperationException("Error al recuperar la categoría recién creada.");
        }

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

        public async Task<CategoriaResponse?> ObtenerCategoriaAsync(int id)
        {
            var categoria = await _db.Categorias
                .Include(c => c.Imagen)
                .Include(c => c.CategoriaPadre)
                .Include(c => c.Subcategorias)
                .Include(c => c.Productos) //solo para contar
                .FirstOrDefaultAsync(c => c.Id == id);

            return categoria is null ? null : MapToResponse(categoria);
        }

        public async Task<List<CategoriaResponse>> ObtenerCategoriasAsync()
        {
            return await _db.Categorias
                .Include(c => c.Imagen)
                .Include(c => c.CategoriaPadre)
                .Include(c => c.Subcategorias)
                .Include(c => c.Productos) //solo para contar
                .Select(c => MapToResponse(c))
                .ToListAsync();
        }

        private static CategoriaResponse MapToResponse(Categoria c) => new()
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            Color = c.Color,
            PopupInformacion = c.PopupInformacion,
            Mayoreo1 = c.Mayoreo1,
            Mayoreo2 = c.Mayoreo2,
            TipoPrecioMayoreo = c.TipoPrecioMayoreo,

            CategoriaPadreId = c.CategoriaPadreId,
            CategoriaPadreNombre = c.CategoriaPadre?.Nombre,

            TotalProductos = c.Productos?.Count ?? 0,

            Imagen = c.Imagen is null ? null : new ImagenResponse()
            { 
                Id = c.Imagen.Id,
                Url = c.Imagen.Url,
                Nombre = c.Imagen.Nombre
            },

            Subcategorias = c.Subcategorias?
                .Where(s => !s.IsDeleted)
                .Select(s => new CategoriaResumenResponse()
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Color = s.Color,
                })
                .ToList() ?? new()
        };
    }
}
