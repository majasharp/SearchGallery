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
    public class GalleryService
    {
        private readonly ILogger<GalleryService> _logger;
        private readonly SearchGalleryDbContext _context;
        public GalleryService(ILogger<GalleryService> logger, SearchGalleryDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<byte[]> GetGalleryItemAsync(Guid guid)
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
