using Catalog.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/product/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService imageService;

        public ImageController(IImageService imageService)
        {
            this.imageService = imageService; 
        }

        [HttpGet("{articul}")]
        public async Task<IActionResult> GetImageProductAsunc(string articul, CancellationToken cancellationToken)
        {
            try
            {
                var result = await this.imageService.GetImageWithAssetsAsync(articul, cancellationToken);

                return PhysicalFile(result, "image/webp");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
