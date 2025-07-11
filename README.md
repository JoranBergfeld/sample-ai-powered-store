# SampleStore Search Agent

This repository holds a sample application that demonstrates how to integrate a chatbot in a .NET application using AI agents. The application showcases a product catalog with search capabilities powered by AI.

## Search Capabilities Comparison: Regex vs. AI Agent

This application showcases two distinct search approaches:

### Regex Search (Products Page)

The regex search on the Products page offers:

- **Pattern-Based Matching**: Searches for specific text patterns in product descriptions
- **Precise Control**: Allows for exact pattern matching using regular expressions
- **Instant Results**: Results update as you type with no API calls required
- **Combined Filtering**: Can be used alongside category filtering

**Use Case Example**: When you know exactly what text you're looking for, like "512GB" or "water.*proof", regex search can quickly filter matching products.

### AI-Powered Search (Agent Page)

In contrast, the AI Agent search provides:

- **Natural Language Understanding**: Interprets what you're looking for even if you don't use exact product terminology
- **Context Awareness**: Understands the intent behind your query, not just the keywords
- **Semantic Matching**: Connects related concepts and synonyms that regex can't capture
- **Multi-Signal Analysis**: Balances the importance of search terms vs. categories based on query analysis

**Use Case Example**: Rather than searching for "mobile AND high-resolution AND portrait", you can simply ask: "I need a smartphone that takes great selfies" and the AI agent will understand and find appropriate products.

### Technical Implementation

The AI search uses Semantic Kernel with Azure OpenAI to:
1. Parse your natural language query
2. Extract key search terms and relevant product categories
3. Determine which factors are more important for your specific query
4. Combine multiple signals to find the most relevant products

## Technologies Used

- **.NET 9**: The application is built on the latest .NET framework
- **Semantic Kernel**: Used for building AI applications with Large Language Models (LLMs)
- **Blazor Server**: For creating interactive web applications using C# instead of JavaScript
- **MudBlazor**: Component library for building rich UI experiences
- **Entity Framework Core**: For data access and in-memory database
- **Azure OpenAI**: Provides the AI capabilities through LLM integration

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- [PowerShell](https://docs.microsoft.com/powershell/scripting/install/installing-powershell)
- An Azure subscription with Azure OpenAI Service access

### Azure OpenAI Configuration

This application requires an Azure OpenAI service to power the AI agent. You'll need to:

1. Create an Azure OpenAI resource in the Azure portal
2. Deploy a model (GPT-4 or GPT-3.5-Turbo recommended) to your Azure OpenAI resource
3. Configure the application with your Azure OpenAI credentials in `appsettings.Development.json`:

```json
{
  "OpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com",
    "DeploymentName": "your-deployment-name",
    "ApiKey": "your-api-key"
  }
}
```

> **Note**: Without valid Azure OpenAI credentials, the application will fall back to mock responses for development purposes only.

### Running Locally

1. Clone the repository:
   ```powershell
   git clone https://github.com/yourusername/samplestore-search-agent.git
   cd samplestore-search-agent
   ```

2. Run the application:
   ```powershell
   cd app
   dotnet run
   ```

3. Open a browser and navigate to `https://localhost:7202` or `http://localhost:5014`

### Project Structure

- **app/**: Contains the main application code
  - **Components/**: Blazor components organized by feature
    - **Layout/**: Application layout components
    - **Pages/**: Page components for different routes
    - **Shared/**: Reusable UI components
  - **Data/**: Database context and configuration
  - **Models/**: Entity classes for the application
  - **Services/**: Business logic and services
  - **wwwroot/**: Static files like CSS and client-side libraries

- **infra/**: Infrastructure as Code using Bicep
  - **main.bicep**: Main deployment module
  - **modules/**: Individual infrastructure components
    - **containerapp.bicep**: Container App deployment
    - **openai.bicep**: Azure OpenAI service deployment

## Deployment to Azure

This application can be deployed to Azure using the Azure Developer CLI (azd). The infrastructure is provisioned using Bicep templates and deployed to Azure Container Apps.

```powershell
# Run the deployment script
./deploy.ps1
```

Alternatively, you can run the deployment steps manually:

```powershell
# Initialize the environment
azd init --environment samplestore

# Provision Azure resources
azd provision

# Deploy the application
azd deploy
```

### Container Deployment with GitHub Actions

This application supports containerized deployment using GitHub Actions and GitHub Packages. For detailed instructions on:
- Building and testing containers locally
- Setting up the GitHub Actions workflow
- Configuring Azure Container Apps to use GitHub Packages

See [Container Deployment Guide](CONTAINER.md) and [Azure Deployment Guide](AZURE_DEPLOYMENT.md)

## Features

- **Product Catalog**: Browse and search for products
- **Tag-Based Filtering**: Filter products by tags/categories
- **Regex Search**: Standard pattern-matching search for product descriptions
- **AI-Powered Search**: Natural language search using Semantic Kernel and Azure OpenAI
- **Responsive UI**: Modern, responsive interface built with MudBlazor components

## Try Both Search Methods

### Testing the Regex Search

1. Navigate to the **Products** page
2. In the "Search by Description" field, try these regex patterns:
   - `\b\d+GB\b` - Finds products with specific storage capacities
   - `water(proof|resistant)` - Finds waterproof or water-resistant products
   - `\b(high|4K)\s+resolution\b` - Finds products with high resolution specifications
3. Notice how results update instantly as you type
4. Try combining with category filters for more refined results

### Testing the AI Agent Search

1. Navigate to the **Agent** page
2. Try these natural language queries:
   - "I need a device for taking professional photos outdoors"
   - "Something comfortable to wear while hiking"
   - "What do you have for cooking that doesn't take much space?"
   - "I want to learn about business but I'm a beginner"
3. Notice how the AI understands the intent behind your query
4. Compare the results with what you'd get using regex search

## How the AI Search Works

The AI-powered product search follows these steps:

1. **User Query Interpretation**: When a user submits a natural language query on the Agent page, the application sends it to the `AIService`.

2. **Prompt Engineering**: The service constructs a prompt that asks the LLM to analyze the query for:
   ```
   You are a helpful shopping assistant for SampleStore. 
   Given the user's query, analyze what they're looking for in terms of:
   - Product characteristics or features
   - Categories or types of products
   - Any specific requirements mentioned
   ```

3. **Semantic Kernel Processing**: The Semantic Kernel sends this prompt to Azure OpenAI, which returns a structured analysis including:
   - `searchTerms`: Array of keywords extracted from the query
   - `categories`: Array of potential product categories
   - `importance`: Whether search terms or categories should be prioritized

4. **JSON Response Cleaning**: The `CleanJsonResponse` method processes the LLM output to ensure it contains only valid JSON, handling common issues like markdown formatting.

5. **Kernel Tool Invocation**: The `ProductSearchTool` plugin is invoked with the structured data to search the product database.

6. **Smart Product Matching**: The tool performs a multi-signal search:
   - Matches product names and descriptions against extracted search terms
   - Matches product tags against identified categories
   - Prioritizes results based on the determined importance (search terms vs. categories)

7. **Results Presentation**: The matching products are returned to the Agent page and displayed to the user.

### Technical Example

For a query like "I need a waterproof device for outdoor photography":

```
User Query → AIService → Semantic Kernel → Azure OpenAI → JSON Response:
{
  "searchTerms": ["waterproof", "device", "outdoor", "photography"],
  "categories": ["electronics", "cameras"],
  "importance": "searchTerms"
}

ProductSearchTool searches:
- Terms match: Products containing "waterproof", "device", "outdoor", or "photography"
- Category match: Products tagged as "electronics" or related to "cameras"
- Prioritization: Term matches displayed first (since importance="searchTerms")
```

This approach combines the precision of programmatic search with the understanding capabilities of large language models.

## Code Organization

The AI search functionality is implemented through:

- **AIService.cs**: Handles AI integration, prompt engineering, and response processing
- **ProductSearchTool.cs**: A Semantic Kernel plugin that implements the product search logic
- **Agent.razor**: The UI component for the natural language search interface
- **Program.cs**: Configuration for Semantic Kernel and Azure OpenAI services

## Development

### Setting Up the Development Environment

1. Open the solution in Visual Studio 2022 or later
2. Restore NuGet packages
3. Configure Azure OpenAI settings in `appsettings.Development.json` (see Azure OpenAI Configuration above)
4. Run the application (F5)

### Project Structure

- **app/**: Contains the main application code
  - **Components/**: Blazor components organized by feature
    - **Pages/Agent.razor**: The AI-powered search interface
    - **Pages/Products.razor**: The regex-based search interface
  - **Services/**: 
    - **AIService.cs**: Core implementation of the AI agent functionality
    - **ProductService.cs**: Database operations for product data
  - **Data/**: Database context and seed data
  - **Models/**: Entity classes for the application

### Extending the AI Capabilities

To enhance the AI agent's capabilities:

1. **Customize the Prompt Template**:
   - Modify the prompt in `AIService.cs` to change how queries are interpreted
   - Add additional analysis requirements to extract more specific information

   ```csharp
   string promptTemplate = @"
   You are a helpful shopping assistant for SampleStore.
   Given the user's query, analyze what they're looking for in terms of:
   - Product characteristics or features
   - Categories or types of products
   - Any specific requirements mentioned
   - [ADD YOUR NEW ANALYSIS POINTS HERE]
   ";
   ```

2. **Enhance the ProductSearchTool**:
   - Add additional search parameters to the `ProductSearchCriteria` class
   - Modify the `FindProducts` method to implement more sophisticated matching algorithms
   - Implement fuzzy matching or semantic similarity for better results

3. **Improve Error Handling**:
   - The `CleanJsonResponse` method can be extended to handle more edge cases
   - Add retry logic for API failures or malformed responses

4. **Add Additional AI Tools**:
   - Create new tools and register them with the Semantic Kernel:
   ```csharp
   _kernel.Plugins.AddFromObject(new YourNewTool(), "ToolName");
   ```

### Adding Your Own Products

The application uses an in-memory database that's seeded with sample products on startup. To add your own products:

1. Modify the database seed logic in `app/Data/AppDbContext.cs`
2. Add new product categories by extending the Tag entities
3. Restart the application to see your changes

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
