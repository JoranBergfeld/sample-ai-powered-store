// search-skillset.bicep
param searchServiceName string
param skillSetName string
param openAiResourceUri string
param openAiApiKey string
param openAiDeploymentId string

resource searchService 'Microsoft.Search/searchServices@2023-11-01' existing = {
  name: searchServiceName
}

resource skillSet 'Microsoft.Search/searchServices/skillsets@2023-11-01' = {
  parent: searchService
  name: skillSetName
  properties: {
    description: 'Skillset to vectorize a CSV column using Azure OpenAI Embedding Skill.'
    skills: [
      {
        '@odata.type': '#Microsoft.Skills.Text.AzureOpenAIEmbeddingSkill'
        name: 'embed-csv-column'
        description: 'Generates embeddings for a specified CSV column.'
        context: '/document'
        resourceUri: openAiResourceUri
        apiKey: openAiApiKey
        deploymentId: openAiDeploymentId
        dimensions: 1536
        modelName: 'text-embedding-3-small'
        inputs: [
          {
            name: 'text'
            source: '/document/Description' // Targetting the Description column
          }
        ]
        outputs: [
          {
            name: 'embedding'
            targetName: 'descriptionVector'
          }
        ]
      }
    ]
  }
}

output skillSetId string = skillSet.id
output skillSetName string = skillSet.name
