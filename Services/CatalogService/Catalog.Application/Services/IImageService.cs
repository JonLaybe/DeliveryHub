namespace Catalog.Application.Services
{
    public interface IImageService
    {
        Task<string> GetImageWithAssetsAsync(string idFolder, CancellationToken cancellationToken = default);
    }
}
