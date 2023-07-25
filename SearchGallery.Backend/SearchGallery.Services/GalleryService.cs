using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeTypes;
using SearchGallery.Persistence;
using SearchGallery.Persistence.Entities;

namespace SearchGallery.Services
{
    public class GalleryService : IGalleryService
    {
        private readonly ILogger<GalleryService> _logger;
        private readonly SearchGalleryDbContext _context;
        private readonly IFileService _fileService;
        private readonly ISearchService _searchService;
        
        public GalleryService(ILogger<GalleryService> logger, SearchGalleryDbContext context, IFileService fileService, ISearchService searchService)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
            _searchService = searchService;
        }

        public async Task<(byte[], string)> GetGalleryItemAsync(Guid guid, bool tryDownloadThumbnail)
        {
            var galleryItem = await _context.GalleryItems.FirstAsync(x => x.Id == guid);

            var extension = tryDownloadThumbnail ? "thumb" : Path.GetExtension(galleryItem.FileName);

            using Stream fileStream = tryDownloadThumbnail 
                ? _fileService.Retrieve(guid, "thumb") 
                : _fileService.Retrieve(guid, extension);

            using var ms = new MemoryStream();
            fileStream.CopyTo(ms);
            return (ms.ToArray(), MimeTypeMap.GetMimeType(extension));
        }

        public async Task<List<GalleryItemDto>> GetGalleryItemsAsync(SearchQuery query)
        {
            if(query.SmartSearchEnabled && !string.IsNullOrEmpty(query.FreeText))
            {
                var allVectors = await _context.GalleryItems.Select(g => new SearchVectorDto
                {
                    Id = g.Id,
                    VectorString = g.Vector
                }).ToListAsync();

                allVectors.ForEach(x => x.ConvertVectorString());
                var searchVector = await _searchService.VectoriseAsync(query.FreeText);

                var ids = _searchService.GetSearchResults(allVectors, searchVector, query.PageCount ?? 10);

                return (await _context.GalleryItems.Where(x => ids.Contains(x.Id)).ToListAsync()).Select(i => new GalleryItemDto
                {
                    FileName = i.FileName,
                    Id = i.Id,
                    SearchText = i.SearchText,
                    UploadedAt = i.UploadedAt,
                    ContentType = i.ContentType
                })
                .ToList();
            }

            return (await _context.GalleryItems
                .Where(x => string.IsNullOrEmpty(query.FreeText) || x.SearchText.ToLower().Contains(query.FreeText.ToLower()))
                .ToListAsync())
                .Select(item => new GalleryItemDto
                {
                    Id = item.Id,
                    FileName = item.FileName,
                    SearchText = item.SearchText,
                    UploadedAt = item.UploadedAt,
                    ContentType = item.ContentType
                })
                .ToList();
        }

        public async Task<GalleryItemDto> UploadGalleryItemAsync(Stream file, string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var contentType = MimeTypeMap.GetMimeType(extension);

            var galleryItem = new GalleryItem
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                UploadedAt = DateTime.Now,
                ContentType = contentType
            };

            var path = await _fileService.StoreAsync(galleryItem.Id, file, Path.GetExtension(fileName));

            galleryItem.SearchText = _searchService.GetSearchText(path);
            galleryItem.Vector = string.Join(";", await _searchService.VectoriseAsync(galleryItem.SearchText));

            await _context.GalleryItems.AddAsync(galleryItem);
            await _context.SaveChangesAsync();

            return new GalleryItemDto
            {
                Id = galleryItem.Id,
                FileName = galleryItem.FileName,
                SearchText = galleryItem.SearchText,
                UploadedAt = galleryItem.UploadedAt,
                ContentType = galleryItem.ContentType
            };
        }

        public async Task DeleteGalleryItemAsync(Guid guid)
        {
            var itemToDelete = await _context.GalleryItems.FirstAsync(x => x.Id == guid);
            _context.GalleryItems.Remove(itemToDelete);
            await _context.SaveChangesAsync();

            _fileService.Delete(guid);
        }
    }
}
