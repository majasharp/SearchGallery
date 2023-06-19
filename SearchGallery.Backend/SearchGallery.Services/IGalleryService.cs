using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchGallery.Services
{
    internal interface IGalleryService
    {
        Task<byte[]> GetGalleryItemAsync(Guid guid, bool tryDownloadThumbnail);
        Task<List<GalleryItemDto>> GetGalleryItemsAsync(SearchQuery query);
        Task<GalleryItemDto> UploadGalleryItemAsync(Stream file);
        Task DeleteGalleryItemAsync(Guid guid);
    }
}
