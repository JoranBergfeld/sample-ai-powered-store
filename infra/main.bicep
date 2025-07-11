targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string = 'swedencentral'

// Optional parameters
param resourceGroupName string = ''
var uniqueResourceNameSuffix = uniqueString(subscription().id, environmentName, location)
var resourceGroupToUse = resourceGroupName != '' ? '${resourceGroupName}-${uniqueResourceNameSuffix}' : 'rg-${environmentName}-${uniqueResourceNameSuffix}'

// Variables to use environment name in resource naming if needed
var tags = {
  environment: environmentName
}

// Create a unique suffix that's shorter to avoid name length issues
var shortUniqueNameSuffix = substring(uniqueString(subscription().id, environmentName, location), 0, 8)

// Parameters for OpenAI module
@description('The name of the Azure OpenAI resource')
param openAiName string = ''
var openAiNameToUse = openAiName != '' ? openAiName : 'oai-${environmentName}-${shortUniqueNameSuffix}'

@description('The SKU name for Azure OpenAI')
param openAiSkuName string = 'S0'

@description('The deployment model for Azure OpenAI')
param openAiDeploymentModel string = 'gpt-4o'

@description('The deployment capacity for Azure OpenAI')
param openAiDeploymentCapacity int = 1

// Parameters for Container App
@description('The name of the container image to deploy')
param containerImage string = 'ghcr.io/yourusername/samplestore-search-agent:latest'

// Create a resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupToUse
  location: location
  tags: tags
}

// Deploy OpenAI module
module openAiModule 'modules/openai.bicep' = {
  name: 'openAiDeploy'
  scope: rg
  params: {
    name: openAiNameToUse
    location: location
    tags: tags
    skuName: openAiSkuName
    deploymentModel: openAiDeploymentModel
    deploymentCapacity: openAiDeploymentCapacity
  }
}

// Deploy Container App module
module containerApp 'modules/containerapp.bicep' = {
  name: 'containerAppDeploy'
  scope: rg
  params: {
    name: 'ss-agent-${shortUniqueNameSuffix}'
    location: location
    tags: tags
    environmentName: 'cae-${environmentName}-${shortUniqueNameSuffix}'
    containerImage: containerImage
    containerAppsEnvironmentId: '' 
  }
}

// Outputs needed for deployment
output AZURE_RESOURCE_GROUP string = rg.name
output AZURE_LOCATION string = location
output AZURE_OPENAI_ENDPOINT string = openAiModule.outputs.endpoint
