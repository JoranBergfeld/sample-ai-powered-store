param searchServiceName string = 'my-product-search-service-${uniqueString(resourceGroup().id)}' 
param location string = resourceGroup().location

// Azure Storage Account Details
param storageAccountName string
@secure()
param storageAccountKey string 
param containerName string = 'products'

// Azure OpenAI Details
param openAiResourceUri string
param openAiDeploymentId string = 'embedding'
@secure()
param openAiApiKey string 

var dataSourceName = 'azureblob-datasource-${uniqueString(resourceGroup().id, storageAccountName, containerName)}'
param indexName string = 'product-index-${uniqueString(resourceGroup().id, storageAccountName, containerName)}'
param skillSetName string = 'csv-embedding-skillset-${uniqueString(resourceGroup().id, storageAccountName, containerName)}'
param indexerName string = 'product-csv-indexer-${uniqueString(resourceGroup().id, storageAccountName, containerName)}'

resource searchService 'Microsoft.Search/searchServices@2023-11-01' = {
  name: searchServiceName
  location: location
  sku: {
    name: 'standard'
  }
  properties: {
    replicaCount: 1
    partitionCount: 1
  }
}

module dataSource 'azureblob-datasource.bicep' = {
  name: 'deploy-datasource'
  params: {
    searchServiceName: searchService.name
    dataSourceName: dataSourceName
    storageAccountName: storageAccountName
    storageAccountKey: storageAccountKey
    containerName: containerName
  }
}

module searchIndex 'search-index.bicep' = {
  name: 'deploy-search-index'
  params: {
    searchServiceName: searchService.name
    indexName: indexName
    openAiResourceUri: openAiResourceUri
    openAiDeploymentId: openAiDeploymentId
    openAiApiKey: openAiApiKey
  }
}

module skillSet 'search-skillset.bicep' = {
  name: 'deploy-skillset'
  params: {
    searchServiceName: searchService.name
    skillSetName: skillSetName
    openAiResourceUri: openAiResourceUri
    openAiApiKey: openAiApiKey
    openAiDeploymentId: openAiDeploymentId
  }
}

module indexer 'search-indexer.bicep' = {
  name: 'deploy-indexer'
  params: {
    searchServiceName: searchService.name
    indexerName: indexerName
    dataSourceName: dataSource.outputs.dataSourceName
    skillsetName: skillSet.outputs.skillSetName
    targetIndexName: searchIndex.outputs.indexName
  }
}
