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
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly SearchGalleryDbContext _context;

        public FileService(ILogger<FileService> logger, SearchGalleryDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async void StoreAsync(Guid guid, Stream stream)
        {

        }

        public async Task<Stream> RetrieveAsync(Guid guid)
        {

        }

        public async void DeleteAsync(Guid guid)
        {

        }

        public async Task MakeThumbnailAsync(Stream stream, string filePath)
        {

        }
    }
}
