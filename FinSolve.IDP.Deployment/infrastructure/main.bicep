param environment string = 'main'
param location string = resourceGroup().location
param prefix string = 'idp'

module eventgrid 'modules/eventgrid.bicep' = {
  name: '${prefix}-eventgrid'
  params: {
    location: location
    storageAccountId: storage.outputs.storageAccountId
    functionAppId: functionapp.outputs.functionAppId
    functionName: 'MetadataValidation'
  }
}

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
