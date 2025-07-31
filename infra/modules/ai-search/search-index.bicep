param searchServiceName string
param indexName string
param openAiResourceUri string
param openAiDeploymentId string

@secure()
param openAiApiKey string 

resource searchService 'Microsoft.Search/searchServices@2023-11-01' existing = {
  name: searchServiceName
}

resource searchIndex 'Microsoft.Search/searchServices/indexes@2023-11-01' = {
  parent: searchService
  name: indexName
  properties: {
    fields: [
      {
        name: 'id'
        type: 'Edm.String'
        searchable: true
        filterable: true
        retrievable: true
        stored: true
        sortable: true
        facetable: true
        key: true
      }
      {
        name: 'Name'
        type: 'Edm.String'
        searchable: true
        filterable: true
        retrievable: true
        stored: true
        sortable: true
        facetable: true
      }
      {
        name: 'Description'
        type: 'Edm.String'
        searchable: true
        filterable: true
        retrievable: true
        stored: true
        sortable: true
        facetable: true
      }
      {
        name: 'descriptionVector'
        type: 'Collection(Edm.Single)'
        searchable: true
        filterable: false
        retrievable: true
        stored: true
        sortable: false
        facetable: false
        dimensions: 1536
        vectorSearchProfile: 'my-product-vector-profile'
      }
    ]
    suggesters: [
      {
        name: 'sg'
        searchMode: 'analyzingInfixMatching'
        sourceFields: [
          'Name'
          'Description'
        ]
      }
    ]
    similarity: {
      '@odata.type': '#Microsoft.Azure.Search.BM25Similarity'
    }
    vectorSearch: {
      algorithms: [
        {
          name: 'hnsw-config'
          kind: 'hnsw'
          hnswParameters: {
            metric: 'cosine'
            m: 4
            efConstruction: 400
            efSearch: 500
          }
        }
      ]
      profiles: [
        {
          name: 'my-product-vector-profile'
          algorithm: 'hnsw-config'
          vectorizer: 'azureOpenAi-text-vectorizer'
        }
      ]
      vectorizers: [
        {
          name: 'azureOpenAi-text-vectorizer'
          kind: 'azureOpenAI'
          azureOpenAIParameters: {
            resourceUri: openAiResourceUri
            deploymentId: openAiDeploymentId
            apiKey: openAiApiKey
            modelName: 'text-embedding-3-small'
          }
        }
      ]
    }
  }
}

output indexId string = searchIndex.id
output indexName string = searchIndex.name
