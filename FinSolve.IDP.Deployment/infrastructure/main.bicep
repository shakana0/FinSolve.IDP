param environment string = 'main'
param location string = resourceGroup().location
param prefix string = 'idp'

module storage 'modules/storage.bicep' = {
  name: '${prefix}storage'
  params: {
    prefix: prefix
    location: location
  }
}

module servicebus 'modules/servicebus.bicep' = {
  name: '${prefix}-servicebus'
  params: {
    prefix: prefix
    location: location
  }
}

module cosmos 'modules/cosmos.bicep' = {
  name: '${prefix}-cosmos'
  params: {
    prefix: prefix
    location: location
  }
}

module keyvault 'modules/keyvault.bicep' = {
  name: '${prefix}-keyvault'
  params: {
    prefix: prefix
    location: location
  }
}

module appinsights 'modules/appinsights.bicep' = {
  name: '${prefix}-appinsights'
  params: {
    prefix: prefix
    location: location
  }
}

module functionapp 'modules/functionapp.bicep' = {
  name: '${prefix}-functionapp'
  params: {
    prefix: prefix
    location: location
    environment: environment
    storageAccountName: storage.outputs.storageAccountName
    serviceBusNamespaceName: servicebus.outputs.serviceBusNamespaceName
    cosmosAccountName: cosmos.outputs.cosmosAccountName
    cosmosDatabaseName: cosmos.outputs.cosmosDatabaseName
    appInsightsName: appinsights.outputs.appInsightsName
    keyVaultName: keyvault.outputs.keyVaultName
  }
}

// --- ROLL-TILLDELNINGAR (RBAC) ---

var storageBlobDataOwnerId = 'b7e69acd-9874-41da-b595-185d17e94d6a'
var serviceBusDataOwnerId = '090c5cfd-751d-490a-8d92-f74d67c0738e'

resource storageRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, '${prefix}-func', storageBlobDataOwnerId)
  scope: resourceGroup()
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataOwnerId)
    principalId: functionapp.outputs.principalId
    principalType: 'ServicePrincipal'
  }
}

resource sbRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, '${prefix}-func', serviceBusDataOwnerId)
  scope: resourceGroup()
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', serviceBusDataOwnerId)
    principalId: functionapp.outputs.principalId
    principalType: 'ServicePrincipal'
  }
}

var kvName = '${prefix}-keyvault'

resource keyVaultResource 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: kvName
}
resource kvAccess 'Microsoft.KeyVault/vaults/accessPolicies@2023-02-01' = {
  parent: keyVaultResource
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: functionapp.outputs.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}
