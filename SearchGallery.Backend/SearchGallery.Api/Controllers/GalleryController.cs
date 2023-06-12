using Microsoft.AspNetCore.Mvc;
using SearchGallery.Persistence.Entities;
using SearchGallery.Services;

namespace SearchGallery.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GalleryController : ControllerBase
    {
        private readonly ILogger<GalleryController> _logger;

        public GalleryController(ILogger<GalleryController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{guid}")]
        public async Task<IActionResult> GetGalleryItem(Guid guid, [FromQuery] bool tryDownloadThumbnail)
        {

        }

        [HttpPost]
        public async Task<IActionResult> GetGalleryItems(SearchQuery query)
        {

        }

        [HttpPost]
        public async Task<IActionResult> UploadGalleryItem(IFormFile file)
        {
            if (file == null) { }
        }

        [HttpDelete("{guid}")]
        public async Task<IActionResult> DeleteGalleryItem(Guid guid)
        {

        }
    }
}