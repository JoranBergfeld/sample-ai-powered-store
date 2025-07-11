@description('The name of the Azure Container App')
param name string

@description('The location for the Azure Container App')
param location string

@description('Tags for the Azure Container App')
param tags object

@description('The name of the Container Apps Environment')
param environmentName string

@description('The name of the container image to deploy')
param containerImage string

@description('CPU cores allocated to a single container instance, e.g., 0.5')
param containerCpuCoreCount string = '0.5'

@description('Memory allocated to a single container instance, e.g., 1Gi')
param containerMemory string = '1Gi'

@description('The minimum number of replicas to run')
param minReplicas int = 1

@description('The maximum number of replicas to run')
param maxReplicas int = 10

@description('Environment variables for the container')
param environmentVariables array = []

@description('Port the container listens on')
param containerPort int = 80

@description('Ingress external or internal')
@allowed([
  'external'
  'internal'
])
param ingressExternal string = 'external'

@description('Ingress target port')
param ingressTargetPort int = containerPort

@description('Container Apps Environment resource ID')
param containerAppsEnvironmentId string

// Create Container App Environment if not provided
resource environment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: environmentName
  location: location
  tags: tags
  properties: {
    zoneRedundant: false
  }
}

// Create Container App
resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: name
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId == '' ? environment.id : containerAppsEnvironmentId
    configuration: {
      ingress: {
        external: ingressExternal == 'external'
        targetPort: ingressTargetPort
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
    }
    template: {
      containers: [
        {
          name: name
          image: containerImage
          resources: {
            cpu: json(containerCpuCoreCount)
            memory: containerMemory
          }
          env: environmentVariables
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

// Outputs
output id string = containerApp.id
output name string = containerApp.name
output fqdn string = containerApp.properties.configuration.ingress.fqdn
output principalId string = containerApp.identity.principalId
output identityTenantId string = containerApp.identity.tenantId
