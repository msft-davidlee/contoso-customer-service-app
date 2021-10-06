param prefix string
param appEnvironment string
param branch string
param location string
param containerName string

var stackName = '${prefix}${appEnvironment}'
var tags = {
  'stack-name': stackName
  'environment': appEnvironment
  'branch': branch
  'team': 'platform'
}

resource acr 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: stackName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    anonymousPullEnabled: false
    policies: {
      retentionPolicy: {
        days: 3
      }
    }
  }
}

resource str 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: stackName
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2021-04-01' = {
  name: 'default'
  parent: str
}

resource blobServiceContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: containerName
  parent: blobService
}

output acrName string = acr.name
output strName string = str.name
