namespace SearchGallery.Services
{
    public class SearchQuery
    {
        public string? FreeText { get; set; }
        public int? Page { get; set; }
        public bool SmartSearchEnabled { get; set; }
        public int? PageCount { get; set; }
    }
}