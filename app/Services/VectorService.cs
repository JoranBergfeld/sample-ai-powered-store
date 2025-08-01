using app.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;

namespace app.Services
{
    public class VectorService
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly AzureAISearchCollection<string, ProductVector> _collection;
        private readonly ProductService _productService;

        public VectorService(
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
            AzureAISearchCollection<string, ProductVector> azureAISearchCollection,
            ProductService productService)
        {
            _embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
            _collection = azureAISearchCollection ?? throw new ArgumentNullException(nameof(azureAISearchCollection));
            _productService = productService;
        }

        public async Task<IEnumerable<Product>> Query(string query, int top = 20)
        {
            var queryEmbedding = await _embeddingGenerator.GenerateVectorAsync(query);
            var products = new List<Product>();
            double thresholdScore = 0.5;
            await foreach (var vectorResult in _collection.SearchAsync(queryEmbedding, top))
            {
                await AddProductFromSearchResultAsync(products, thresholdScore, vectorResult);
            }

            return products;
        }

        private async Task AddProductFromSearchResultAsync(List<Product> products, double thresholdScore, VectorSearchResult<ProductVector> vectorResult)
        {
            if (vectorResult != null &&
                                vectorResult.Score.HasValue &&
                                vectorResult.Score.Value > thresholdScore &&
                                int.TryParse(vectorResult.Record.Id, out int productId) &&
                                await _productService.GetProductAsync(productId) is Product product)
            {
                products.Add(product);
            }
        }
    }

    public record ProductVector
    {
        [VectorStoreKey]
        public string Id { get; set; }

        [VectorStoreData]
        public string Description { get; set; }

        [VectorStoreVector(1536)]
        public ReadOnlyMemory<float> descriptionVector { get; set; }
    }
}
