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
        private readonly IGalleryService _service;

        public GalleryController(ILogger<GalleryController> logger, IGalleryService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("{guid}")]
        public async Task<IActionResult> GetGalleryItem(Guid guid, [FromQuery] bool tryDownloadThumbnail)
        {
            var result = await _service.GetGalleryItemAsync(guid, tryDownloadThumbnail);
            return File(result.Item1, result.Item2);
        }

        [HttpPost]
        public async Task<IActionResult> GetGalleryItems(SearchQuery query)
        {
            var result = await _service.GetGalleryItemsAsync(query);
            return Ok(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadGalleryItem(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var result = await _service.UploadGalleryItemAsync(stream, file.FileName);
            return Ok(result);
        }

        [HttpDelete("{guid}")]
        public async Task<IActionResult> DeleteGalleryItem(Guid guid)
        {
            await _service.DeleteGalleryItemAsync(guid);
            return Ok();
        }
    }
}