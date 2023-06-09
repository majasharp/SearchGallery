﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchGallery.Services
{
    public interface IGalleryService
    {
        Task<(byte[], string)> GetGalleryItemAsync(Guid guid, bool tryDownloadThumbnail);
        Task<List<GalleryItemDto>> GetGalleryItemsAsync(SearchQuery query);
        Task<GalleryItemDto> UploadGalleryItemAsync(Stream file, string fileName);
        Task DeleteGalleryItemAsync(Guid guid);
    }
}
