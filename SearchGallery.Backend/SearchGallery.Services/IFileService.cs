using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchGallery.Services
{
    public interface IFileService
    {
        Task<string> StoreAsync(Guid guid, Stream stream, string extension);
        Stream Retrieve(Guid guid, string extension);
        void Delete(Guid guid);
        Task MakeThumbnailAsync(Stream stream, string filePath);
    }
}
