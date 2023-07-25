using Microsoft.Extensions.Logging;
using System.Drawing;
using Microsoft.Extensions.Options;

namespace SearchGallery.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly StorageConfiguration _config;

        public FileService(ILogger<FileService> logger, IOptions<StorageConfiguration> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async Task<string> StoreAsync(Guid guid, Stream stream, string extension)
        {
            var path = Path.ChangeExtension($"{_config.BaseDirectory}/{guid}", extension);
            var outgoingStream = File.OpenWrite(path);
            try
            {
                await stream.CopyToAsync(outgoingStream);
            }
            finally
            {
                await outgoingStream.DisposeAsync();
            }

            await MakeThumbnailAsync(File.OpenRead(path), path);
            return path;
        }

        public Stream Retrieve(Guid guid, string extension)
        {
            var path = Path.ChangeExtension($"{_config.BaseDirectory}/{guid}", extension);
            return File.OpenRead(path);
        }

        public void Delete(Guid guid)
        {
            var files = Directory.GetFiles(_config.BaseDirectory, $"{guid}.*");
            foreach (var f in files)
            {
                File.Delete(f);
            }
        }

        private async Task MakeThumbnailAsync(Stream stream, string path)
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
