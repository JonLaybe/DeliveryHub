using Catalog.Application.Services;
using Shared.Domain.Exceptions;

namespace Catalog.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        public Task<string> GetImageWithAssetsAsync(string idFolder, CancellationToken cancellationToken = default)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string pathToImages = Path.Combine(webRootPath, "images", idFolder);

            string imagePath = Path.Combine(pathToImages, "main.webp");

            if (!File.Exists(imagePath))
                throw new NotFoundEntityException();

            return Task.FromResult(imagePath);
        }
    }
}
