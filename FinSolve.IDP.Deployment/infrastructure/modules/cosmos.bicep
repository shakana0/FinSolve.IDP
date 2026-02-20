param prefix string
param location string
param databaseName string = 'idp-db'

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: '${prefix}-cosmos'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    enableFreeTier: false
    enableAnalyticalStorage: false
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
  }
}

resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: cosmosAccount
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource processingResultsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: cosmosDatabase
  name: 'processingResults'
  properties: {
    resource: {
      id: 'processingResults'
      partitionKey: {
        paths: ['/documentId']
        kind: 'Hash'
      }
    }
  }
}

resource documentStatusContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: cosmosDatabase
  name: 'documentStatus'
  properties: {
    resource: {
      id: 'documentStatus'
      partitionKey: {
        paths: ['/documentId']
        kind: 'Hash'
      }
    }
  }
}

resource deadLettersContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: cosmosDatabase
  name: 'deadLetters'
  properties: {
    resource: {
      id: 'deadLetters'
      partitionKey: {
        paths: ['/messageId']
        kind: 'Hash'
      }
    }
  }
}

output cosmosAccountName string = cosmosAccount.name
output cosmosDatabaseName string = databaseName
output cosmosAccountId string = cosmosAccount.id
