using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchGallery.Services
{
    public interface IFileService
    {
        void StoreAsync(Guid guid, Stream stream);
        Task<Stream> RetrieveAsync(Guid guid);
        void DeleteAsync(Guid guid);
        Task MakeThumbnailAsync(Stream stream, string filePath);
    }
}
