metadata description = 'Creates an Azure SQL Database server and database.'

@minLength(1)
@maxLength(63)
@description('The name of the SQL server')
param sqlServerName string

@minLength(1)
@maxLength(128)
@description('The name of the SQL database')
param sqlDatabaseName string

@description('The location where the SQL server will be deployed')
param location string = resourceGroup().location

@description('Tags to apply to the SQL server and database')
param tags object = {}

@description('The administrator login name for the SQL server')
param administratorLogin string

@secure()
@description('The administrator password for the SQL server')
param administratorPassword string

@description('The tier of the database SKU')
@allowed(['Basic', 'Standard', 'Premium', 'GeneralPurpose', 'BusinessCritical'])
param databaseSkuTier string = 'Basic'

@description('The name of the database SKU')
param databaseSkuName string = 'Basic'

@description('The maximum size of the database in bytes')
param maxSizeBytes int = 2147483648 // 2GB default for Basic tier

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    version: '12.0'
    publicNetworkAccess: 'Enabled'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  tags: tags
  sku: {
    name: databaseSkuName
    tier: databaseSkuTier
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: maxSizeBytes
  }
}

// Allow Azure services to access the server
resource firewallRuleAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output sqlServerName string = sqlServer.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output sqlDatabaseName string = sqlDatabase.name