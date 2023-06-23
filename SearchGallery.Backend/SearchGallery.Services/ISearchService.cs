using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchGallery.Services
{
    public interface ISearchService
    {
        string GetSearchText(string path);
        Task<float[]> VectoriseAsync(string searchText);
    }
}
