using app.Data;
using app.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Azure.AI.Vision.ImageAnalysis;
using System.Collections.Generic;

namespace app.Services;

public class AIService
{
    private readonly Kernel _kernel;
    private readonly ILogger<AIService> _logger;
    private readonly AppDbContext _dbContext;
    private readonly ProductService _productService;
    private readonly ImageAnalysisClientFactory _visionClientFactory;

    public AIService(
        Kernel kernel, 
        ILogger<AIService> logger,
        AppDbContext dbContext,
        ProductService productService,
        ImageAnalysisClientFactory visionClientFactory)
    {
        _kernel = kernel;
        _logger = logger;
        _dbContext = dbContext;
        _productService = productService;
        _visionClientFactory = visionClientFactory;
        
        // Register the product search tool
        _kernel.Plugins.AddFromObject(new ProductSearchTool(_productService), "ProductSearch");
    }

    public async Task<List<Product>> SearchProductsByNaturalLanguageAsync(string query)
    {
        try
        {
            _logger.LogInformation("Processing natural language query: {Query}", query);
            
            // Use the kernel function to find products directly
            var arguments = new KernelArguments
            {
                ["query"] = query
            };

            var searchFunction = _kernel.Plugins["ProductSearch"]["FindProducts"];
            var matchingProducts = await _kernel.InvokeAsync<List<Product>>(searchFunction, arguments);

            return matchingProducts ?? new List<Product>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing natural language product search");
            return new List<Product>();
        }
    }

    public async Task<ImageAnalysisResult> AnalyzeImageAsync(byte[] imageData)
    {
        try
        {
            _logger.LogInformation("Processing image analysis request");
            
            // Create a vision client
            var visionClient = _visionClientFactory.CreateClient();
            
            // Set up the visual features for analysis
            var visualFeatures = VisualFeatures.Caption | VisualFeatures.Objects | VisualFeatures.Tags;
            
            // Create a BinaryData object from the image bytes
            var binaryData = new BinaryData(imageData);
            
            // Analyze the image
            var response = await visionClient.AnalyzeAsync(binaryData, visualFeatures);
            
            // Get the result from the response
            var visionResult = response.Value;

            _logger.LogInformation("Azure Vision API analysis completed");

            // Parse results into our ImageAnalysisResult model
            var result = new ImageAnalysisResult
            {
                Description = visionResult.Caption?.Text ?? "No description available",
                Objects = ExtractObjects(visionResult),
                Characteristics = ExtractCharacteristics(visionResult),
                SuggestedCategories = ExtractCategories(visionResult)
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image analysis: {Message}", ex.Message);
            return new ImageAnalysisResult
            {
                Description = $"Error analyzing image: {ex.Message}",
                Objects = Array.Empty<string>(),
                Characteristics = Array.Empty<string>(),
                SuggestedCategories = Array.Empty<string>()
            };
        }
    }

    private string[] ExtractObjects(Azure.AI.Vision.ImageAnalysis.ImageAnalysisResult visionResult)
    {
        var objects = new List<string>();
        
        // Add detected objects if available
        if (visionResult.Objects != null)
        {
            // Extract detected objects
            foreach (var detectedObject in visionResult.Objects.Values)
            {
                // Each detected object may have tags, add the first tag as the object name
                if (detectedObject.Tags.Any())
                {
                    var objectName = detectedObject.Tags.First().Name;
                    if (!string.IsNullOrEmpty(objectName) && !objects.Contains(objectName))
                    {
                        objects.Add(objectName);
                    }
                }
            }
        }
        
        return objects.ToArray();
    }

    private string[] ExtractCharacteristics(Azure.AI.Vision.ImageAnalysis.ImageAnalysisResult visionResult)
    {
        var characteristics = new List<string>();
        
        // Add characteristics from tags if available
        if (visionResult.Tags != null)
        {
            // Extract tags that represent characteristics (colors, materials, etc.)
            foreach (var tag in visionResult.Tags.Values)
            {
                // Filter tags to include only those that describe characteristics
                // and have confidence above 0.7
                if (tag.Confidence >= 0.7 && IsCharacteristic(tag.Name))
                {
                    characteristics.Add(tag.Name);
                }
            }
        }
        
        return characteristics.ToArray();
    }

    private string[] ExtractCategories(Azure.AI.Vision.ImageAnalysis.ImageAnalysisResult visionResult)
    {
        var categories = new List<string>();
        
        // Add potential categories from tags with higher confidence if available
        if (visionResult.Tags != null)
        {
            // Extract tags that might represent product categories
            foreach (var tag in visionResult.Tags.Values)
            {
                // Use tags with high confidence as potential categories
                if (tag.Confidence >= 0.8 && IsCategory(tag.Name))
                {
                    categories.Add(tag.Name);
                }
            }
        }
        
        return categories.ToArray();
    }

    // Helper method to identify tags that represent characteristics
    private bool IsCharacteristic(string tagName)
    {
        // Common color names
        string[] colors = { "red", "blue", "green", "yellow", "black", "white", "brown", "orange", "purple", "pink", "gray" };
        
        // Common materials
        string[] materials = { "wood", "metal", "plastic", "glass", "fabric", "leather", "cotton", "wool", "silk", "ceramic" };
        
        // Common shapes and styles
        string[] styles = { "round", "square", "rectangular", "modern", "vintage", "casual", "formal", "elegant", "rustic", "minimalist" };

        // Check if the tag is in any of these categories
        return colors.Contains(tagName.ToLower()) || 
               materials.Contains(tagName.ToLower()) || 
               styles.Contains(tagName.ToLower());
    }

    // Helper method to identify tags that represent potential product categories
    private bool IsCategory(string tagName)
    {
        // Common product categories
        string[] categories = { 
            "furniture", "electronics", "clothing", "appliance", "device", "accessory", "tool", 
            "kitchenware", "home", "office", "sport", "toy", "book", "game", "computer", "phone", 
            "camera", "watch", "shoe", "bag", "dress", "shirt", "pant", "jacket", "coat", "food"
        };

        return categories.Contains(tagName.ToLower());
    }

    public async Task<List<Product>> FindProductsFromImageAnalysisAsync(ImageAnalysisResult analysisResult)
    {
        try
        {
            // Create a simple query from the image analysis
            var queryParts = new List<string>();
            queryParts.AddRange(analysisResult.Objects);
            queryParts.AddRange(analysisResult.Characteristics);
            queryParts.AddRange(analysisResult.SuggestedCategories);
            
            var query = string.Join(" ", queryParts.Distinct());
            
            // Use the simplified search function
            return await SearchProductsByNaturalLanguageAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding products from image analysis");
            return new List<Product>();
        }
    }

    private string CleanJsonResponse(string text)
    {
        // If the text is null or empty, return an empty JSON object
        if (string.IsNullOrWhiteSpace(text))
        {
            return "{}";
        }

        // Remove any markdown code block syntax (```json and ```)
        text = Regex.Replace(text, @"```json\s*", "");
        text = Regex.Replace(text, @"```\s*", "");

        // Find the first { and the last } to extract just the JSON object
        int firstBrace = text.IndexOf('{');
        int lastBrace = text.LastIndexOf('}');

        if (firstBrace != -1 && lastBrace != -1 && lastBrace > firstBrace)
        {
            // Extract just the JSON part
            text = text.Substring(firstBrace, lastBrace - firstBrace + 1);
        }
        else
        {
            // If we can't find valid JSON braces, return an empty object
            return "{}";
        }

        return text;
    }
}

// Class to hold image analysis results
public class ImageAnalysisResult
{
    public string Description { get; set; } = "";
    public string[] Objects { get; set; } = Array.Empty<string>();
    public string[] Characteristics { get; set; } = Array.Empty<string>();
    public string[] SuggestedCategories { get; set; } = Array.Empty<string>();
}

// Tool to search for products that will be used by the kernel
public class ProductSearchTool
{
    private readonly ProductService _productService;

    public ProductSearchTool(ProductService productService)
    {
        _productService = productService;
    }

    [KernelFunction]
    [Description("Find products based on user query")]
    public async Task<List<Product>> FindProducts(
        [Description("User query to match against product names and descriptions")] string query)
    {
        // Get all products
        var allProducts = await _productService.GetProductsAsync();
        
        // If query is empty, return empty list
        if (string.IsNullOrWhiteSpace(query))
            return new List<Product>();
        
        // Split query into words for matching
        var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        // Find products that match any word in the query
        var matchingProducts = allProducts.Where(p => 
            queryWords.Any(word => 
                p.Name.Contains(word, StringComparison.OrdinalIgnoreCase) || 
                p.Description.Contains(word, StringComparison.OrdinalIgnoreCase)
            )).ToList();
        
        return matchingProducts;
    }
}