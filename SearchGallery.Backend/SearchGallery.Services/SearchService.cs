using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Embedding;
using SearchGallery.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace SearchGallery.Services
{
    public class SearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly SearchGalleryDbContext _context;
        private readonly OpenAIAPI? _api;

        public SearchService(ILogger<SearchService> logger, SearchGalleryDbContext context)
        {
            _logger = logger;
            _context = context;
            var openAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            if (!string.IsNullOrEmpty(openAIKey))
            {
                _api = new OpenAIAPI(openAIKey);
            }
        }

        public string GetSearchText(string path)
        {
            using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            using var img = Pix.LoadFromFile(path);
            using var page = engine.Process(img);
            return page.GetText();
        }

        public async Task<float[]> VectoriseAsync(string searchText)
        {
            if(_api == null)
            {
                throw new ArgumentNullException(nameof(_api));
            }
            var embedding = await _api.Embeddings.CreateEmbeddingAsync(new EmbeddingRequest
            {
                Input = searchText,
                Model = "text-embedding-ada-002" // text-davinci-003
            });

            return embedding.Data.First(x => x.Embedding?.Any() ?? false).Embedding;
        }

        public List<Guid> GetSearchResults(IList<SearchVectorDto> allVectors, float[] searchVector, int numberOfResults)
        {
            var similarities = allVectors
            .OrderByDescending(x => GetCosineSimilarity(x.Vector, searchVector))
            .ToList();

            return similarities.Take(numberOfResults).Select(x => x.Id).ToList();
        }

        private static double GetCosineSimilarity(float[] V1, float[] V2)
        {
            var N = Math.Min(V1.Length, V2.Length);
            var dotProduct = 0.0;
            var magnitude1 = 0.0;
            var magnitude2 = 0.0;

            for (var n = 0; n < N; n++)
            {
                dotProduct += V1[n] * V2[n];
                magnitude1 += Math.Pow(V1[n], 2);
                magnitude2 += Math.Pow(V2[n], 2);
            }

            return dotProduct / (Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
        }
    }
}
