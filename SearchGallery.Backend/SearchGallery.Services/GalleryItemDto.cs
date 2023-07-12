using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchGallery.Services
{
    public class GalleryItemDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string SearchText { get; set; }
        public DateTime UploadedAt { get; set; }
        public string ContentType { get; set; }
    }
}
