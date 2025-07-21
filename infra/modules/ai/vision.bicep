@description('The name of the Azure Computer Vision resource')
param name string

@description('Tags for the Azure Computer Vision resource')
param tags object

@description('The SKU name for Azure Computer Vision')
param skuName string = 'S1'

// Location is hardcoded to westeurope as required for caption functionality
param location string = 'westeurope'

resource vision 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: name
  location: location
  tags: tags
  kind: 'ComputerVision'
  sku: {
    name: skuName
  }
  properties: {
    customSubDomainName: name
    publicNetworkAccess: 'Enabled'
  }
}

// Outputs
output id string = vision.id
output name string = vision.name
output endpoint string = vision.properties.endpoint
output key1 string = listKeys(vision.id, '2023-05-01').key1
