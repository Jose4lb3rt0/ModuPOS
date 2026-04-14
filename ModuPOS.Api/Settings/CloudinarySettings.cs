namespace ModuPOS.Api.Settings
{
    public class CloudinarySettings
    {
        public const string Section = "CloudinarySettings"; //como en appsettings.json

        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }
}
