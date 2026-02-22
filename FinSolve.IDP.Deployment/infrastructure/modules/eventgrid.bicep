param location string
param storageAccountId string
param functionAppId string
param functionName string = 'MetadataValidation'

resource systemTopic 'Microsoft.EventGrid/systemTopics@2022-06-15' = {
  name: 'idp-blob-uploads'
  location: location
  properties: {
    source: storageAccountId
    topicType: 'Microsoft.Storage.StorageAccounts'
  }
}

resource eventSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2022-06-15' = {
  parent: systemTopic
  name: 'idp-metadata-sub'
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '${functionAppId}/functions/${functionName}'
      }
    }
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
      subjectBeginsWith: '/blobServices/default/containers/documents/blobs/'
    }
  }
}
