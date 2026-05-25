namespace Chat.Api.Configs
{
    public class ServicesConfig
    {
        public CatalogApi? CatalogApi { get; set; }
    }

    public class CatalogApi
    {
        public string BaseUrl { get; set; } = string.Empty;
    }
}
