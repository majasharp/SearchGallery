using Microsoft.EntityFrameworkCore;
using SearchGallery.Persistence.Entities;

namespace SearchGallery.Persistence
{
    public class SearchGalleryDbContext : DbContext
    {
        public SearchGalleryDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<GalleryItem> GalleryItems { get; set; }
    }
}
