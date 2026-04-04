using ModuPOS.Shared.Models;

namespace ModuPOS.Api.Entities
{
    public class MetodoPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
