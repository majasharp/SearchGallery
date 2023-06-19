using Microsoft.Extensions.Logging;
using SearchGallery.Persistence;
using SearchGallery.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchGallery.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly ILogger<GalleryService> _logger;
        private readonly SearchGalleryDbContext _context;
        
        public GalleryService(ILogger<GalleryService> logger, SearchGalleryDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<byte[]> GetGalleryItemAsync(Guid guid, bool tryDownloadThumbnail)
        {

        }

        public async Task<List<GalleryItemDto>> GetGalleryItemsAsync(SearchQuery query)
        {

        }

        public async Task<GalleryItemDto> UploadGalleryItemAsync(Stream file)
        {

        }

        public async Task DeleteGalleryItemAsync(Guid guid)
        {

        }
    }
}
