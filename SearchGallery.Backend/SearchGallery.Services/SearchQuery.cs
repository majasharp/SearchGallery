using System.ComponentModel.DataAnnotations;

namespace SearchGallery.Services
{
    public class SearchQuery
    {
        public string? FreeText { get; set; }
        public int? Page { get; set; }
    }
}