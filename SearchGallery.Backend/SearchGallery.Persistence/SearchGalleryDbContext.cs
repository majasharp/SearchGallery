using Microsoft.EntityFrameworkCore;
using SearchGallery.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
