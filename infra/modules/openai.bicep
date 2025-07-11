@description('The name of the Azure OpenAI resource')
param name string

@description('The location for the Azure OpenAI resource')
param location string

@description('Tags for the Azure OpenAI resource')
param tags object

@description('The SKU name for Azure OpenAI')
param skuName string = 'S0'

@description('The deployment model for Azure OpenAI')
param deploymentModel string = 'gpt-35-turbo'

@description('The deployment capacity for Azure OpenAI')
param deploymentCapacity int = 1

@description('Deployment name for the model')
param deploymentName string = 'chat'

resource openAi 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
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

resource openAiDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
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

// Outputs
output id string = openAi.id
output name string = openAi.name
output endpoint string = openAi.properties.endpoint
