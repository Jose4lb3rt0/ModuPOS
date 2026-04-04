using ModuPOS.Shared.Models;

namespace ModuPOS.Api.Entities
{
    public class MetodoPago : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
