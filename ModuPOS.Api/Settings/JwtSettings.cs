namespace ModuPOS.Api.Settings
{
    public class JwtSettings
    {
        public const string Section = "JwtSettings";

        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 480; //8 horas de sesión
    }
}
