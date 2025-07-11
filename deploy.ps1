# Deploy infrastructure and application to Azure

param (
    [string]$environmentName = "samplestore",
    [string]$location = "swedencentral",
    [switch]$container = $false,
    [string]$containerImage = ""
)

# Check if Azure Developer CLI is installed
try {
    $azdVersion = (azd version) 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Azure Developer CLI not found or returned an error."
    }
    Write-Host "Using Azure Developer CLI version: $azdVersion"
} catch {
    Write-Host "Error: Azure Developer CLI (azd) is not installed or not in the PATH." -ForegroundColor Red
    Write-Host "Please install Azure Developer CLI from https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd" -ForegroundColor Red
    exit 1
}

# Check if the user is logged in and select subscription
Write-Host "Checking Azure login status..."
try {
    # Check if logged in
    $loginCheck = az account show 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Not logged in to Azure. Logging in now..." -ForegroundColor Yellow
        az login
        if ($LASTEXITCODE -ne 0) {
            throw "Azure login failed."
        }
    }
    
    # List subscriptions and let the user select one
    Write-Host "Available subscriptions:" -ForegroundColor Cyan
    $subscriptions = az account list --query "[].{Name:name, Id:id, IsDefault:isDefault}" -o json | ConvertFrom-Json
    
    for ($i=0; $i -lt $subscriptions.Length; $i++) {
        $defaultMarker = if ($subscriptions[$i].IsDefault) { " (default)" } else { "" }
        Write-Host "[$i] $($subscriptions[$i].Name) - $($subscriptions[$i].Id)$defaultMarker"
    }
    
    $selectedIndex = Read-Host "Enter the number of the subscription to use"
    $selectedSubscription = $subscriptions[$selectedIndex]
    
    Write-Host "Setting subscription to: $($selectedSubscription.Name)" -ForegroundColor Cyan
    az account set --subscription $selectedSubscription.Id
    
    # Also set it for azd
    azd config set defaults.subscription $selectedSubscription.Id
    
} catch {
    Write-Host "Error setting Azure subscription: $_" -ForegroundColor Red
    exit 1
}

# Initialize the environment with Azure Developer CLI if not already initialized
Write-Host "Checking environment initialization..."
$env = azd env list | Select-String -Pattern $environmentName
if (!$env) {
    Write-Host "Initializing environment..."
    azd env new $environmentName --location $location
} else {
    Write-Host "Environment $environmentName already initialized."
    azd env select $environmentName
}

# Set required environment variables
Write-Host "Setting environment variables..."
azd env set AZURE_LOCATION $location
azd env set location $location
azd env set environmentName $environmentName
Write-Host "Set deployment location to: $location"
Write-Host "Set environment name to: $environmentName"

# Set container variables if deploying a container
if ($container) {
    Write-Host "Setting up container deployment..."
    if ([string]::IsNullOrEmpty($containerImage)) {
        $containerImage = "ghcr.io/yourusername/samplestore-search-agent:latest"
        Write-Host "Container image not specified. Using default GitHub Packages image: $containerImage"
    }
} else {
    # Set default container image even if not explicitly using container mode
    if ([string]::IsNullOrEmpty($containerImage)) {
        $containerImage = "ghcr.io/yourusername/samplestore-search-agent:latest"
    }
}
azd env set containerImage $containerImage
Write-Host "Set container image to: $containerImage"

# Provision Azure resources
Write-Host "Provisioning Azure resources..." -ForegroundColor Cyan
try {
    # Create or update parameters file
    $parametersContent = @"
{
  "`$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "environmentName": {
      "value": "$environmentName"
    },
    "location": {
      "value": "$location"
    },
    "containerImage": {
      "value": "$containerImage"
    }
  }
}
"@
    $parametersPath = "infra/main.parameters.json"
    Set-Content -Path $parametersPath -Value $parametersContent
    Write-Host "Updated parameters file at: $parametersPath" -ForegroundColor Cyan
    
    # Run the provision command
    $provisionOutput = azd provision --no-prompt
    if ($LASTEXITCODE -ne 0) {
        throw "Provisioning failed. Check the output for details."
    }
    Write-Host "Azure resources provisioned successfully." -ForegroundColor Green
} catch {
    Write-Host "Error during provisioning: $_" -ForegroundColor Red
    Write-Host "Provisioning output: $provisionOutput" -ForegroundColor Red
    exit 1
}

# Deploy the application
Write-Host "Deploying application..." -ForegroundColor Cyan
try {
    $deployOutput = azd deploy --no-prompt
    if ($LASTEXITCODE -ne 0) {
        throw "Deployment failed. Check the output for details."
    }
    Write-Host "Application deployed successfully." -ForegroundColor Green
} catch {
    Write-Host "Error during deployment: $_" -ForegroundColor Red
    Write-Host "Deployment output: $deployOutput" -ForegroundColor Red
    exit 1
}

Write-Host "Deployment completed successfully!" -ForegroundColor Green

# Output the application URL
Write-Host "Getting application URL..." -ForegroundColor Cyan
try {
    # First try to use the azd command to get the URL
    $endpoints = azd pipeline endpoint list
    if ($endpoints -match "https://") {
        $url = $endpoints | Where-Object { $_ -match "https://" } | Select-Object -First 1
        Write-Host "Application URL: $url" -ForegroundColor Green
    } else {
        # Fall back to Azure CLI if azd doesn't return the URL
        $rg = az group list --query "[?contains(name, '$environmentName')].name" -o tsv
        if (-not [string]::IsNullOrEmpty($rg)) {
            # Get all container apps in the resource group and find the one with our app name
            $containerApps = az containerapp list --resource-group $rg --query "[?starts_with(name, 'ss-agent-')].name" -o tsv
            if ($containerApps) {
                $appName = $containerApps
                $url = az containerapp show --name $appName --resource-group $rg --query "properties.configuration.ingress.fqdn" -o tsv
                if (-not [string]::IsNullOrEmpty($url)) {
                    Write-Host "Application URL: https://$url" -ForegroundColor Green
                } else {
                    Write-Host "Could not retrieve the application URL. Please check the Azure portal." -ForegroundColor Yellow
                }
            } else {
                Write-Host "Could not find the container app. Please check the Azure portal." -ForegroundColor Yellow
            }
        } else {
            Write-Host "Could not find the resource group. Please check the Azure portal." -ForegroundColor Yellow
        }
    }
} catch {
    Write-Host "Could not retrieve the application URL: $_" -ForegroundColor Yellow
    Write-Host "Please check the Azure portal to find your deployed application." -ForegroundColor Yellow
}

Write-Host "To check application logs, use: az containerapp logs show --name <container-app-name> --resource-group <resource-group-name>" -ForegroundColor Cyan
Write-Host "You can find your container app name in the Azure portal or by running: az containerapp list --resource-group <resource-group-name> --query [].name -o tsv" -ForegroundColor Cyan
