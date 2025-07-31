metadata description = 'Creates an Azure AI Search service with indexer configured to link to a SQL Database'

@minLength(2)
@maxLength(60)
@description('The name of the Azure AI Search service')
param searchServiceName string

@description('The location where the search service will be deployed')
param location string = resourceGroup().location

@description('Tags to apply to the search service')
param tags object = {}

@description('The SKU of the search service')
@allowed(['free', 'basic', 'standard', 'standard2', 'standard3'])
param searchServiceSku string = 'basic'

@description('The number of replicas for the search service')
@minValue(1)
@maxValue(12)
param replicaCount int = 1

@description('The number of partitions for the search service')
@allowed([1, 2, 3, 4, 6, 12])
param partitionCount int = 1

@description('The connection string for the SQL database')
@secure()
param sqlConnectionString string

// Azure AI Search service
resource searchService 'Microsoft.Search/searchServices@2023-11-01' = {
  name: searchServiceName
  location: location
  tags: tags
  sku: {
    name: searchServiceSku
  }
  properties: {
    replicaCount: replicaCount
    partitionCount: partitionCount
    hostingMode: 'default'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Data source for SQL Database
resource dataSource 'Microsoft.Search/searchServices/datasources@2023-11-01' = {
  parent: searchService
  name: 'products-datasource'
  properties: {
    type: 'azuresql'
    credentials: {
      connectionString: sqlConnectionString
    }
    container: {
      name: 'Products'
    }
    dataChangeDetectionPolicy: {
      '@odata.type': '#Microsoft.Azure.Search.HighWaterMarkChangeDetectionPolicy'
      highWaterMarkColumnName: 'Id'
    }
  }
}

// Search index for products
resource searchIndex 'Microsoft.Search/searchServices/indexes@2023-11-01' = {
  parent: searchService
  name: 'products-index'
  properties: {
    fields: [
      {
        name: 'Id'
        type: 'Edm.String'
        key: true
        searchable: false
        filterable: true
        sortable: true
        facetable: false
        retrievable: true
      }
      {
        name: 'Name'
        type: 'Edm.String'
        key: false
        searchable: true
        filterable: true
        sortable: true
        facetable: false
        retrievable: true
        analyzer: 'standard.lucene'
      }
      {
        name: 'Description'
        type: 'Edm.String'
        key: false
        searchable: true
        filterable: false
        sortable: false
        facetable: false
        retrievable: true
        analyzer: 'standard.lucene'
      }
    ]
    corsOptions: {
      allowedOrigins: ['*']
    }
  }
}

// Indexer to populate the search index from SQL database
resource indexer 'Microsoft.Search/searchServices/indexers@2023-11-01' = {
  parent: searchService
  name: 'products-indexer'
  properties: {
    dataSourceName: dataSource.name
    targetIndexName: searchIndex.name
    schedule: {
      interval: 'PT1H' // Run every hour
    }
    parameters: {
      configuration: {
        dataToExtract: 'contentAndMetadata'
      }
    }
    fieldMappings: [
      {
        sourceFieldName: 'Id'
        targetFieldName: 'Id'
        mappingFunction: {
          name: 'base64Encode'
        }
      }
      {
        sourceFieldName: 'Name'
        targetFieldName: 'Name'
      }
      {
        sourceFieldName: 'Description'
        targetFieldName: 'Description'
      }
    ]
  }
  dependsOn: [
    dataSource
    searchIndex
  ]
}

// Outputs
output searchServiceName string = searchService.name
output searchServiceId string = searchService.id
output searchServiceEndpoint string = 'https://${searchService.name}.search.windows.net'
output searchServiceAdminKey string = searchService.listAdminKeys().primaryKey
output searchServiceQueryKey string = searchService.listQueryKeys().value[0].key