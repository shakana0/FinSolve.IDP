param prefix string
param location string

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: '${prefix}-kv'
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    enableSoftDelete: true
    enablePurgeProtection: false
    accessPolicies: []
  }
}

output keyVaultName string = keyVault.name
output keyVaultId string = keyVault.id
