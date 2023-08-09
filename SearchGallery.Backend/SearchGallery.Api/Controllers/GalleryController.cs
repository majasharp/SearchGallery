using Microsoft.AspNetCore.Mvc;
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
            try
            {
                var result = await _service.GetGalleryItemAsync(guid, tryDownloadThumbnail);
                return File(result.Item1, result.Item2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in the retrieval of a GalleryItem with id: {guid}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGalleryItems(SearchQuery query)
        {
            try
            {
                var result = await _service.GetGalleryItemsAsync(query);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in the retrieval of multiple GalleryItems with SearchQuery: {query.FreeText}");
                return StatusCode(500);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadGalleryItem(IFormFile file)
        {
            try
            {
                var stream = file.OpenReadStream();
                var result = await _service.UploadGalleryItemAsync(stream, file.FileName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the upload of a file.");
                return StatusCode(500);
            }
        }

        [HttpDelete("{guid}")]
        public async Task<IActionResult> DeleteGalleryItem(Guid guid)
        {
            try
            {
                await _service.DeleteGalleryItemAsync(guid);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in the deletion of GalleryItem with id: {guid}");
                return StatusCode(500);
            }
        }
    }
}