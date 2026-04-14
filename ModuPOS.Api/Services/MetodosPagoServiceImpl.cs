using Microsoft.EntityFrameworkCore;
using ModuPOS.Api.Data;
using ModuPOS.Api.Entities;
using ModuPOS.Shared.DTOs.MetodoPago;

namespace ModuPOS.Api.Services
{
    public class MetodosPagoServiceImpl : IMetodosPagoService
    {
        private readonly ModuPosDbContext _context;

        public MetodosPagoServiceImpl(ModuPosDbContext context)
        {
            _context = context;
        }

        public async Task<MetodoPagoResponse> CrearMetodoPagoAsync(CrearMetodoPagoRequest request)
        {
            if (await _context.MetodosPago.AnyAsync(m => m.Nombre.ToLower() == request.Nombre.ToLower()))
            {
                throw new InvalidOperationException($"El método de pago '{request.Nombre}' ya existe.");
            }

            var nuevoMetodo = new MetodoPago { Nombre = request.Nombre };

            _context.MetodosPago.Add(nuevoMetodo);
            await _context.SaveChangesAsync();

            return new MetodoPagoResponse { Id = nuevoMetodo.Id, Nombre = nuevoMetodo.Nombre };
        }

        public async Task<List<MetodoPagoResponse>> ObtenerMetodosPagoAsync()
        {
            return await _context.MetodosPago
                .Select(m => new MetodoPagoResponse
                {
                    Id = m.Id,
                    Nombre = m.Nombre
                })
                .ToListAsync();
        }
    }
}
