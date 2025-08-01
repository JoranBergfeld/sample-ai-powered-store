@description('The name of the storage account')
param name string

@description('The location for the storage account')
param location string

@description('Tags for the storage account')
param tags object

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    publicNetworkAccess: 'Enabled'
  }

  resource blobService 'blobServices@2023-01-01' = {
    name: 'default'
    
    resource container 'containers@2023-01-01' = {
      name: 'products'
      properties: {
        publicAccess: 'None'
      }
    }
  }
}

output id string = storageAccount.id
output name string = storageAccount.name
@secure()
output primaryKey string = storageAccount.listKeys().keys[0].value
