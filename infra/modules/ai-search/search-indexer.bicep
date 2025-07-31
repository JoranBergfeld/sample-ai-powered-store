param searchServiceName string
param indexerName string
param dataSourceName string
param skillsetName string
param targetIndexName string

resource searchService 'Microsoft.Search/searchServices@2023-11-01' existing = {
  name: searchServiceName
}

resource indexer 'Microsoft.Search/searchServices/indexers@2023-11-01' = {
  parent: searchService
  name: indexerName
  properties: {
    dataSourceName: dataSourceName
    skillsetName: skillsetName
    targetIndexName: targetIndexName
    parameters: {
      configuration: {
        parsingMode: 'delimitedText'
        firstLineContainsHeaders: true
      }
    }
    fieldMappings: [
      {
        sourceFieldName: 'Id'
        targetFieldName: 'id'
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
    outputFieldMappings: [
      {
        sourceFieldName: '/document/descriptionVector'
        targetFieldName: 'descriptionVector'
      }
    ]
  }
}

output indexerId string = indexer.id
output indexerName string = indexer.name
