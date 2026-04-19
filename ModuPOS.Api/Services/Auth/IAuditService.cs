namespace ModuPOS.Api.Services.Auth
{
    public interface IAuditService
    {
        //devuelve el id del usuario autenticado o null si no hay sesión
        string? ObtenerUsuarioActual();
    }
}
