using Microsoft.Extensions.Logging;
using OpenAI_API;
using OpenAI_API.Embedding;
using SearchGallery.Persistence;
using System.Collections.Concurrent;
using Tesseract;

namespace SearchGallery.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly OpenAIAPI? _api;

        public SearchService(ILogger<SearchService> logger, SearchGalleryDbContext context)
        {
            _logger = logger;
            var openAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            if (!string.IsNullOrEmpty(openAIKey))
            {
                _api = new OpenAIAPI(openAIKey);
            }
        }

        public string GetSearchText(string path)
        {
            try
            {
                using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
                using var img = Pix.LoadFromFile(path);
                using var page = engine.Process(img);

                // log the file path to the image and the Tesseract cean confidence score
                _logger.LogInformation($"{path} : {page.GetMeanConfidence()}");

                return page.GetText();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"An error occurred while extracting text with Tesseract for path: {path}.");
                throw;
            }
        }

        public async Task<float[]> VectoriseAsync(string searchText)
        {
            try
            {
                if (_api == null)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while text embedding with OpenAI for: {searchText}.");
                throw;
            }
        }

        public List<Guid> GetSearchResults(IList<SearchVectorDto> allVectors, float[] searchVector, int numberOfResults)
        {
            var similarities = new ConcurrentBag<(double, Guid)>();

            Parallel.ForEach(allVectors, x =>
            {
                var similarity = GetCosineSimilarity(x.Vector, searchVector);
                similarities.Add((similarity, x.Id));
            });

            var results = similarities.OrderByDescending(x => x.Item1).Take(numberOfResults).ToList();

            // log guid of item with corresponding cosine similarity to the search term
            foreach (var result in results)
            {
                _logger.LogInformation($"{result.Item2} : {result.Item1}");
            }
            
            return similarities.OrderByDescending(x => x.Item1).Take(numberOfResults).Select(x => x.Item2).ToList();
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
