param prefix string
param appEnvironment string
param branch string
param location string

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

output acrName string = acr.name
