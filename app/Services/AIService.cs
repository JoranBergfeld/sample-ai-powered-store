using app.Data;
using app.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace app.Services;

public class AIService
{
    private readonly Kernel _kernel;
    private readonly ILogger<AIService> _logger;
    private readonly AppDbContext _dbContext;
    private readonly ProductService _productService;

    public AIService(
        Kernel kernel, 
        ILogger<AIService> logger,
        AppDbContext dbContext,
        ProductService productService)
    {
        _kernel = kernel;
        _logger = logger;
        _dbContext = dbContext;
        _productService = productService;
        
        // Register the product search tool
        _kernel.Plugins.AddFromObject(new ProductSearchTool(_productService), "ProductSearch");
    }

    public async Task<List<Product>> SearchProductsByNaturalLanguageAsync(string query)
    {
        try
        {
            _logger.LogInformation("Processing natural language query: {Query}", query);
            
            // Define the prompt template to process user queries about products
            string promptTemplate = @"
            You are a helpful shopping assistant for SampleStore. 
            Given the user's query, analyze what they're looking for in terms of:
            - Product characteristics or features
            - Categories or types of products
            - Any specific requirements mentioned

            User query: {{$query}}

            Based on this query, extract the key search terms and categories that would help find relevant products.
            Respond with a JSON object with these properties:
            - searchTerms: array of strings representing important keywords from the query
            - categories: array of possible category names that might match what the user is looking for
            - importance: indicate which is more important for this query - the search terms or the categories - as a string either 'searchTerms' or 'categories'

            Return only valid JSON without any markdown formatting or backticks:";

            // Create the prompt with the user's query
            var arguments = new KernelArguments
            {
                ["query"] = query
            };

            // Execute the prompt using the kernel
            var result = await _kernel.InvokePromptAsync(promptTemplate, arguments);
            var resultText = result.GetValue<string>() ?? string.Empty;

            _logger.LogInformation("AI Processing result: {Result}", resultText);

            // Clean the result text - ensure it only contains valid JSON
            resultText = CleanJsonResponse(resultText);

            // Parse the JSON response
            ProductSearchCriteria? searchCriteria;
            try
            {
                searchCriteria = JsonSerializer.Deserialize<ProductSearchCriteria>(resultText, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (searchCriteria == null)
                {
                    _logger.LogWarning("Failed to parse AI response to search criteria");
                    return new List<Product>();
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error with text: {Text}", resultText);
                // Create a fallback search criteria with basic terms from the query
                searchCriteria = new ProductSearchCriteria
                {
                    SearchTerms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries),
                    Categories = Array.Empty<string>(),
                    Importance = "searchTerms"
                };
            }

            // Use the search criteria to find products
            var arguments2 = new KernelArguments
            {
                ["searchTerms"] = string.Join(" ", searchCriteria.SearchTerms),
                ["categories"] = string.Join(" ", searchCriteria.Categories),
                ["importance"] = searchCriteria.Importance
            };

            var searchFunction = _kernel.Plugins["ProductSearch"]["FindProducts"];
            var matchingProducts = await _kernel.InvokeAsync<List<Product>>(searchFunction, arguments2);

            return matchingProducts ?? new List<Product>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing natural language product search");
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

// Class to hold parsed search criteria from AI
public class ProductSearchCriteria
{
    public string[] SearchTerms { get; set; } = Array.Empty<string>();
    public string[] Categories { get; set; } = Array.Empty<string>();
    public string Importance { get; set; } = "searchTerms";
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
    [Description("Find products based on search terms and/or categories")]
    public async Task<List<Product>> FindProducts(
        [Description("Space-separated search terms to match against product descriptions")] string searchTerms,
        [Description("Space-separated category names to filter products")] string categories,
        [Description("Which is more important for this query - 'searchTerms' or 'categories'")] string importance)
    {
        // Get all products first
        var allProducts = await _productService.GetProductsAsync();
        
        // Split the search terms and categories
        var terms = searchTerms.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var categoryNames = categories.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        // Filter by search terms
        var termMatches = allProducts.Where(p => 
            terms.Any(term => 
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) || 
                p.Description.Contains(term, StringComparison.OrdinalIgnoreCase)
            )).ToList();
            
        // Filter by categories
        var categoryMatches = allProducts.Where(p =>
            p.ProductTags.Any(pt => 
                categoryNames.Any(cat => 
                    pt.Tag.Name.Contains(cat, StringComparison.OrdinalIgnoreCase)
                )
            )).ToList();
            
        // Determine which results to prioritize based on the importance
        if (importance.Equals("searchTerms", StringComparison.OrdinalIgnoreCase))
        {
            // If search terms are more important, return term matches first, then add any category matches not already included
            var result = new List<Product>(termMatches);
            foreach (var product in categoryMatches)
            {
                if (!result.Any(p => p.Id == product.Id))
                {
                    result.Add(product);
                }
            }
            return result;
        }
        else
        {
            // If categories are more important, return category matches first, then add any term matches not already included
            var result = new List<Product>(categoryMatches);
            foreach (var product in termMatches)
            {
                if (!result.Any(p => p.Id == product.Id))
                {
                    result.Add(product);
                }
            }
            return result;
        }
    }
}