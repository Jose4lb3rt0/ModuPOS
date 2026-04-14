namespace ModuPOS.Api.Entities
{
    public class Imagen : BaseEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ProveedorId { get; set; } = string.Empty;
        public string Proveedor { get; set; } = "Cloudinary"; //"Cloudinary" o "LocalStorage"
    }
}
