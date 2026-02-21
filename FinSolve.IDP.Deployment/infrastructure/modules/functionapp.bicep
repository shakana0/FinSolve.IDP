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

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' existing = {
  name: 'default'
  parent: storageAccount
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

// --- ROLE-DEFINITIONS ---

var monitoringRoleID = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  '3913584d-2f98-4d3b-953e-7db0026df405'
)
var serviceBusRoleID = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  '090c5cfd-751d-490a-8d92-f74d67c0738e'
)
var storageContributorRoleID = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  '17d1049b-9a84-46fb-8f53-86981c22a3f4'
)
var storageBlobRoleID = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  'b7e69acd-9874-41da-b595-185d17e94d6a'
)
var cosmosDataContributorRoleID = '00000000-0000-0000-0000-000000000002'

// ---  ROLE ASSIGNMENTS ---

// 1. Application Insights
resource appInsightsRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(appInsights.id, functionApp.id, monitoringRoleID)
  scope: appInsights
  properties: {
    roleDefinitionId: monitoringRoleID
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// 2. Service Bus
resource sbAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(serviceBusNamespace.id, functionApp.id, serviceBusRoleID)
  scope: serviceBusNamespace
  properties: {
    roleDefinitionId: serviceBusRoleID
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// 3. Storage Account
resource storageAccountAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, functionApp.id, storageContributorRoleID)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageContributorRoleID
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// 4. Storage Blob
resource storageAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, functionApp.id, storageBlobRoleID)
  scope: storageAccount // Ändrat till storageAccount för enkelhet, eller behåll blobService om den är definierad
  properties: {
    roleDefinitionId: storageBlobRoleID
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// 5. Cosmos DB
resource cosmosAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2023-04-15' = {
  name: guid(cosmosAccount.id, functionApp.id, cosmosDataContributorRoleID)
  parent: cosmosAccount
  properties: {
    roleDefinitionId: resourceId(
      'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions',
      cosmosAccount.name,
      cosmosDataContributorRoleID
    )
    principalId: functionApp.identity.principalId
    scope: cosmosAccount.id
  }
}

// 6. Key Vault Access Policy
resource kvAccess 'Microsoft.KeyVault/vaults/accessPolicies@2023-02-01' = {
  name: 'add'
  parent: keyVault
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: functionApp.identity.principalId
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

output functionAppId string = functionApp.id
