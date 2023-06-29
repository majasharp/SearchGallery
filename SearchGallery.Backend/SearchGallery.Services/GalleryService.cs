using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SearchGallery.Persistence;
using SearchGallery.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<byte[]> GetGalleryItemAsync(Guid guid, bool tryDownloadThumbnail)
        {
            Stream fileStream = null;
            if(tryDownloadThumbnail)
            {
                fileStream = _fileService.Retrieve(guid, "thumb");
            }
            else
            {
                var galleryItem = await _context.GalleryItems.FirstAsync(x => x.Id == guid);
                fileStream = _fileService.Retrieve(guid, Path.GetExtension(galleryItem.FileName));
            }

            using var ms = new MemoryStream();
            fileStream.CopyTo(ms);
            return ms.ToArray();
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
                    UploadedAt = i.UploadedAt
                })
                .ToList();
            }

            return (await _context.GalleryItems
                .Where(x => string.IsNullOrEmpty(query.FreeText) || x.SearchText.Contains(query.FreeText))
                .ToListAsync())
                .Select(item => new GalleryItemDto
                {
                    Id = item.Id,
                    FileName = item.FileName,
                    SearchText = item.SearchText,
                    UploadedAt = item.UploadedAt
                })
                .ToList();
        }

        public async Task<GalleryItemDto> UploadGalleryItemAsync(Stream file, string fileName)
        {
            var galleryItem = new GalleryItem
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                UploadedAt = DateTime.Now
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
