param prefix string
param location string
param topicName string = 'idp-documents'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: '${prefix}-sb-namespace'
  location: location
  sku: {
    name: 'Standard'
  }
}

resource topic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = {
  parent: serviceBusNamespace
  name: 'idp-events'
}

// 1. Metadata Validated Subscription & Rule
resource metadataValidatedSub 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01-preview' = {
  parent: topic
  name: 'metadata-validated-sub'
  properties: {
    maxDeliveryCount: 5
    deadLetteringOnMessageExpiration: true
  }
}

resource metadataValidatedRule 'Microsoft.ServiceBus/namespaces/topics/subscriptions/rules@2022-10-01-preview' = {
  parent: metadataValidatedSub
  name: 'metadata-validated-rule'
  properties: {
    filterType: 'SqlFilter'
    sqlFilter: {
      sqlExpression: 'sys.Label = \'MetadataValidated\''
    }
  }
}

// 2. Processing Completed Subscription & Rule
resource processingCompletedSub 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01-preview' = {
  parent: topic
  name: 'processing-completed-sub'
  properties: {
    maxDeliveryCount: 5
    deadLetteringOnMessageExpiration: true
  }
}

resource processingCompletedRule 'Microsoft.ServiceBus/namespaces/topics/subscriptions/rules@2022-10-01-preview' = {
  parent: processingCompletedSub
  name: 'processing-completed-rule'
  properties: {
    filterType: 'SqlFilter'
    sqlFilter: {
      sqlExpression: 'sys.Label = \'ProcessingCompleted\''
    }
  }
}

// 3. Summary Created Subscription & Rule
resource summaryCreatedSub 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01-preview' = {
  parent: topic
  name: 'summary-created-sub'
  properties: {
    maxDeliveryCount: 5
    deadLetteringOnMessageExpiration: true
  }
}

resource summaryCreatedRule 'Microsoft.ServiceBus/namespaces/topics/subscriptions/rules@2022-10-01-preview' = {
  parent: summaryCreatedSub
  name: 'summary-created-rule'
  properties: {
    filterType: 'SqlFilter'
    sqlFilter: {
      sqlExpression: 'sys.Label = \'SummaryCreated\''
    }
  }
}

// 4. PDF Generated Subscription & Rule
resource pdfGeneratedSub 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01-preview' = {
  parent: topic
  name: 'pdf-generated-sub'
  properties: {
    maxDeliveryCount: 5
    deadLetteringOnMessageExpiration: true
  }
}

resource pdfGeneratedRule 'Microsoft.ServiceBus/namespaces/topics/subscriptions/rules@2022-10-01-preview' = {
  parent: pdfGeneratedSub
  name: 'pdf-generated-rule'
  properties: {
    filterType: 'SqlFilter'
    sqlFilter: {
      sqlExpression: 'sys.Label = \'PdfGenerated\''
    }
  }
}

output serviceBusNamespaceName string = serviceBusNamespace.name
output topicNameOut string = topicName
output serviceBusNamespaceId string = serviceBusNamespace.id
