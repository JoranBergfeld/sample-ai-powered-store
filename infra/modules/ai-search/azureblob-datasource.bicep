param searchServiceName string
param dataSourceName string
param storageAccountName string
@secure()
param storageAccountKey string
param containerName string

resource searchService 'Microsoft.Search/searchServices@2023-11-01' existing = {
  name: searchServiceName
}

resource dataSource 'Microsoft.Search/searchServices/dataSources@2023-11-01' = {
  parent: searchService
  name: dataSourceName
  properties: {
    description: 'Data source for CSV files in Azure Blob Storage'
    type: 'azureblob'
    credentials: {
      connectionString: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${storageAccountKey}'
    }
    container: {
      name: containerName
    }
  }
}

output dataSourceId string = dataSource.id
output dataSourceName string = dataSource.name
