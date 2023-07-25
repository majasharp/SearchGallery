namespace SearchGallery.Services
{
    public interface ISearchService
    {
        string GetSearchText(string path);
        Task<float[]> VectoriseAsync(string searchText);
        List<Guid> GetSearchResults(IList<SearchVectorDto> allVectors, float[] searchVector, int numberOfResults);
    }
}
