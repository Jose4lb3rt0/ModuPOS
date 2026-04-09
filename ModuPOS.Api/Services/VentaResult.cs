using ModuPOS.Shared.DTOs;

namespace ModuPOS.Api.Services
{
    public class VentaResult
    {
        private VentaResult() { }

        public bool EsExitoso { get; private set; }
        public string? MensajeError { get; private set; }
        public VentaResponse? Datos { get; private set; }

        public static VentaResult Ok(VentaResponse datos) =>
            new() { EsExitoso = true, Datos = datos };

        public static VentaResult Fail(string error) =>
            new() { EsExitoso = false, MensajeError = error };
    }
}
