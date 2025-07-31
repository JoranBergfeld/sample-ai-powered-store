targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string
@minLength(1)
@description('Location for AI Search and Storage resources')
// Constrained due to semantic ranker availability: https://learn.microsoft.com/azure/search/search-region-support#americas
@allowed([
  'northeurope'
  'francecentral'
  'switzerlandnorth'
  'switzerlandwest'
  'uksouth'
])
@metadata({
  azd: {
    type: 'location'
  }
})
param location string

@description('Location for the OpenAI resource group')
@allowed([
  'eastus2'
  'swedencentral'
])
@metadata({
  azd: {
    type: 'location'
  }
})
param openAiServiceLocation string

var uniqueResourceNameSuffix = uniqueString(subscription().id, environmentName, location)
var baseSuffix = '${environmentName}-${uniqueResourceNameSuffix}'
var baseSuffixShortend = substring(uniqueString(subscription().id, environmentName, location), 0, 8)
var abbrs = loadJsonContent('abbreviations.json')
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var tags = { 'azd-env-name': environmentName }

// Names of resources which could be passed
param resourceGroupName string = ''
param logAnalyticsName string = ''
param backendServiceName string = ''

@description('The workload profile for Azure Container Apps')
param acaIdentityName string = '${environmentName}-aca-identity'

@description('The name of the Azure OpenAI resource')
param openAiName string = ''
var openAiNameToUse = openAiName != '' ? openAiName : 'oai-${baseSuffixShortend}'

@description('The SKU name for Azure OpenAI')
param openAiSkuName string = 'S0'

@description('The deployment model for Azure OpenAI')
param openAiDeploymentModel string = 'gpt-4o'

@description('The deployment capacity for Azure OpenAI')
param openAiDeploymentCapacity int = 1

@description('The name of the Azure Computer Vision resource')
param visionName string = ''
var visionNameToUse = visionName != '' ? visionName : '${abbrs.cognitiveServicesComputerVision}${baseSuffix}'

@description('The SKU name for Azure Computer Vision')
param visionSkuName string = 'S1'

@description('The name of the Azure Container Registry')
param containerRegistryName string = ''

@allowed(['Consumption', 'D4', 'D8', 'D16', 'D32', 'E4', 'E8', 'E16', 'E32', 'NC24-A100', 'NC48-A100', 'NC96-A100'])
param azureContainerAppsWorkloadProfile string

@description('Used by azd for containerapps deployment')
param webAppExists bool

@description('The name of the Azure Storage Account')
param storageAccountName string = ''

resource rg 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: !empty(resourceGroupName) ? resourceGroupName : '${abbrs.resourcesResourceGroups}${environmentName}'
  location: location
  tags: tags
}

module logAnalytics 'br/public:avm/res/operational-insights/workspace:0.7.0' = {
  name: 'loganalytics'
  scope: rg
  params: {
    name: !empty(logAnalyticsName) ? logAnalyticsName : '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    location: location
    tags: tags
    skuName: 'PerGB2018'
    dataRetention: 30
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    useResourcePermissions: true
  }
}

// Deploy OpenAI module
module openAiModule 'modules/ai/openai.bicep' = {
  name: 'openAiDeploy'
  scope: rg
  params: {
    name: openAiNameToUse
    location: openAiServiceLocation
    tags: tags
    skuName: openAiSkuName
    deploymentModel: openAiDeploymentModel
    deploymentCapacity: openAiDeploymentCapacity
    embeddingModel: 'text-embedding-3-small'
    embeddingDeploymentName: 'embedding'
  }
}

// Deploy Computer Vision module - always in westeurope for caption functionality
module visionModule 'modules/ai/vision.bicep' = {
  name: 'visionDeploy'
  scope: rg
  params: {
    name: visionNameToUse
    tags: tags
    skuName: visionSkuName
    // Location is hardcoded in the module to westeurope
  }
}

module storageAccountModule 'modules/storage/storage-account.bicep' = {
  name: 'storageAccountDeploy'
  scope: rg
  params: {
    name: storageAccountName
    location: location
    tags: tags
  }
}

module aiSearch 'modules/ai-search/ai-search.bicep' = {
  name: 'aiSearchWithStorageDeploy'
  scope: rg
  params: {
    storageAccountName: storageAccountModule.outputs.name
    storageAccountKey: storageAccountModule.outputs.primaryKey
    location: location
    openAiResourceUri: openAiModule.outputs.endpoint
    openAiDeploymentId: openAiModule.outputs.embeddingModelName
    openAiApiKey: openAiModule.outputs.key1
  }
}

module acaIdentity 'modules/security/aca-identity.bicep' = {
  name: 'aca-identity'
  scope: rg
  params: {
    identityName: acaIdentityName
    location: location
  }
}

module containerApps 'modules/host/container-apps.bicep' = {
  name: 'container-apps'
  scope: rg
  params: {
    name: 'app'
    tags: tags
    location: location
    workloadProfile: azureContainerAppsWorkloadProfile
    containerAppsEnvironmentName: '${environmentName}-aca-env'
    containerRegistryName: '${containerRegistryName}${resourceToken}'
    logAnalyticsWorkspaceResourceId: logAnalytics.outputs.resourceId
  }
}

module acaBackend 'modules/host/container-app-upsert.bicep' = {
  name: 'aca-web'
  scope: rg
  params: {
    name: !empty(backendServiceName) ? backendServiceName : '${abbrs.webSitesContainerApps}${resourceToken}'
    location: location
    identityName: acaIdentityName
    exists: webAppExists
    workloadProfile: azureContainerAppsWorkloadProfile
    containerRegistryName: containerApps.outputs.registryName
    containerAppsEnvironmentName: containerApps.outputs.environmentName
    identityType: 'UserAssigned'
    tags: union(tags, { 'azd-service-name': 'app' })
    targetPort: 80
    containerCpuCoreCount: '2.0'
    containerMemory: '4Gi'
    env: {
      OpenAI__Endpoint: openAiModule.outputs.endpoint
      OpenAI__DeploymentName: openAiModule.outputs.modelName
      OpenAI__ApiKey: openAiModule.outputs.key1
      AzureVision__Endpoint: visionModule.outputs.endpoint
      AzureVision__ApiKey: visionModule.outputs.key1
    }
  }
}

// Outputs needed for deployment
output AZURE_RESOURCE_GROUP string = rg.name
output AZURE_LOCATION string = location
output AZURE_OPENAI_ENDPOINT string = openAiModule.outputs.endpoint
output AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME string = openAiModule.outputs.embeddingModelName
output AZURE_VISION_ENDPOINT string = visionModule.outputs.endpoint
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerApps.outputs.registryLoginServer
output AZURE_STORAGE_ACCOUNT_NAME string = storageAccountModule.outputs.name
