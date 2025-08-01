@description('The name of the Azure OpenAI resource')
param name string

@description('The location for the Azure OpenAI resource')
param location string

@description('Tags for the Azure OpenAI resource')
param tags object

@description('The SKU name for Azure OpenAI')
param skuName string = 'S0'

@description('The deployment model for Azure OpenAI')
param deploymentModel string = 'gpt-4o'

@description('The deployment capacity for Azure OpenAI')
param deploymentCapacity int = 1

@description('Deployment name for the model')
param deploymentName string = 'chat'

@description('The embedding model for Azure OpenAI')
param embeddingModel string = 'text-embedding-3-small'

@description('The embedding deployment capacity for Azure OpenAI')
param embeddingDeploymentCapacity int = 1

@description('Deployment name for the embedding model')
param embeddingDeploymentName string = 'embedding'

resource openAi 'Microsoft.CognitiveServices/accounts@2025-06-01' = {
  name: name
  location: location
  tags: tags
  kind: 'OpenAI'
  sku: {
    name: skuName
  }
  properties: {
    customSubDomainName: name
    publicNetworkAccess: 'Enabled'
  }
}

resource openAiDeployment 'Microsoft.CognitiveServices/accounts/deployments@2025-06-01' = {
  parent: openAi
  name: deploymentName
  tags: tags
  sku: {
    name: 'Standard'
    capacity: deploymentCapacity
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: deploymentModel
    }
  }
}

resource openAiEmbeddingDeployment 'Microsoft.CognitiveServices/accounts/deployments@2025-06-01' = {
  parent: openAi
  name: embeddingDeploymentName
  tags: tags
  sku: {
    name: 'Standard'
    capacity: embeddingDeploymentCapacity
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: embeddingModel
    }
  }
  dependsOn: [
    openAiDeployment
  ]
}

// Outputs
output id string = openAi.id
output name string = openAi.name
output endpoint string = openAi.properties.endpoint
output modelName string = deploymentName
output embeddingModelName string = embeddingDeploymentName
@secure()
output key1 string = openAi.listKeys().key1
