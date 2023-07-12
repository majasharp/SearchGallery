namespace SearchGallery.Persistence.Entities
{
    public class GalleryItem
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string SearchText { get; set; }
        public string? Vector { get; set; }
        public DateTime UploadedAt { get; set; }
        public string ContentType { get; set; }
    }
}