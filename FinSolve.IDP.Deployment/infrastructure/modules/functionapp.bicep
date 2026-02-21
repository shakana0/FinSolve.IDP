param prefix string
param location string
param environment string

// Inputs from other modules
param storageAccountName string
param serviceBusNamespaceName string
param cosmosAccountName string
param cosmosDatabaseName string
param appInsightsName string
param keyVaultName string

// --- EXISTING RESOURCES ---

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' existing = {
  name: serviceBusNamespaceName
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: cosmosAccountName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}

// Function App Plan
resource functionPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${prefix}-func-plan'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

// Function App
resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: '${prefix}-func'
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: functionPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AZURE_FUNCTIONS_ENVIRONMENT'
          value: environment
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'dev' ? 'Development' : 'Production'
        }
        {
          name: 'AzureWebJobsStorage__accountName'
          value: storageAccount.name
        }
        {
          name: 'AzureWebJobsStorage__credential'
          value: 'managedidentity'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'StorageAccountName'
          value: storageAccount.name
        }
        {
          name: 'KeyVaultUri'
          value: keyVault.properties.vaultUri
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        // Important: Tell SDK to use Managed Identity to send logs
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_BaseExtensions'
          value: 'disabled'
        }
        // Recource connections
        {
          name: 'ServiceBusConnection__fullyQualifiedNamespace'
          value: '${serviceBusNamespace.name}.servicebus.windows.net'
        }
        {
          name: 'CosmosDbAccountEndpoint'
          value: cosmosAccount.properties.documentEndpoint
        }
        {
          name: 'Cosmos__Database'
          value: cosmosDatabaseName
        }
        {
          name: 'Cosmos__StatusContainer'
          value: 'documentStatus'
        }
        {
          name: 'Cosmos__ResultContainer'
          value: 'processingResults'
        }
        {
          name: 'Cosmos__DlqContainer'
          value: 'deadLetters'
        }
      ]
    }
  }
}

output functionAppId string = functionApp.id
output principalId string = functionApp.identity.principalId
