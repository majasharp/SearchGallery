using Microsoft.Extensions.Logging;
using SearchGallery.Persistence;
using SearchGallery.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Extensions.Options;

namespace SearchGallery.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly SearchGalleryDbContext _context;
        private readonly StorageConfiguration _config;

        public FileService(ILogger<FileService> logger, SearchGalleryDbContext context, IOptions<StorageConfiguration> config)
        {
            _logger = logger;
            _context = context;
            _config = config.Value;
        }

        public async Task<string> StoreAsync(Guid guid, Stream stream, string extension)
        {
            var path = $"{_config.BaseDirectory}/{guid}.{extension}";
            await using var outgoingStream = File.OpenWrite(path);
            await stream.CopyToAsync(outgoingStream);

            return path;
        }

        public Stream Retrieve(Guid guid, string extension)
        {
            return File.OpenRead($"{_config.BaseDirectory}/{guid}.{extension}");
        }

        public void Delete(Guid guid)
        {
            var files = Directory.GetFiles(_config.BaseDirectory, $"{guid}.*");
            foreach (var f in files)
            {
                File.Delete(f);
            }
        }

        public async Task MakeThumbnailAsync(Stream stream, string path)
        {
            var thumbPath = Path.ChangeExtension(path, "thumb");

            try
            {
                var image = Image.FromStream(stream);
                image = ThumbnailResizer.ResizeImage(image, 120, 120);
                var thumb = image.GetThumbnailImage(image.Width, image.Height, () => false, IntPtr.Zero);
                thumb.Save(thumbPath);
                thumb.Dispose();

                File.SetAttributes(thumbPath, FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Thumbnail can't be made for file: {thumbPath}.", thumbPath);
            }
        }
    }
}
