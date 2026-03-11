🎯 Architectural Strategy & FinOps – FinSolve IDP
This document explains the technical "Why" behind the FinSolve IDP. It bridges the gap between the requirements and the final implementation, with a heavy focus on resilience and cloud economy.

1. FinOps: Cost Optimization Strategy
   To meet NFR-1 and NFR-2, the system is built on a "Pay-per-use" model. We have eliminated all fixed monthly costs, ensuring the infrastructure scales its cost linearly with the volume of processed documents.

| Service             | SKU / Tier       | Optimization Strategy                                                                                                         |
| :------------------ | :--------------- | :---------------------------------------------------------------------------------------------------------------------------- |
| **Azure Functions** | Consumption (Y1) | **Zero Idle Cost:** Vi betalar endast för de millisekunder koden faktiskt körs. Skalar automatiskt till noll vid inaktivitet. |
| **Cosmos DB**       | Serverless       | **No Provisioned Throughput:** Eliminerar kostnaden för oanvända RU/s. Perfekt för oförutsägbara finansiella flöden.          |
| **Service Bus**     | Standard         | **Enterprise Pub/Sub:** Möjliggör Topics och SQL-filter till en fast låg kostnad, vilket undviker den dyra Premium-tiern.     |
| **Storage Account** | Standard (LRS)   | **Hot Tier:** Optimerad för låg latens vid läsning/skrivning under den aktiva bearbetningsfasen.                              |
| **App Insights**    | Log Analytics    | **Smart Sampling:** Konfigurerad till 5% sampling för att behålla full spårbarhet men minimera kostnader för datalagring.     |

2. Risk Mitigation & System Resiliency
   Architecture is not just about the "Happy Path." Based on our risk analysis, the following patterns have been implemented:
   2.1 Load Leveling (The Buffer Pattern)
   To satisfy NFR-1 (Elastic Scale), we use Azure Service Bus as a managed buffer.
   • Challenge: A sudden spike of 5,000 uploads could overwhelm downstream processing or hit Cosmos DB rate limits.
   • Solution: Service Bus holds the messages and allows the Function App to process them at a controlled, sustainable rate.
   2.2 Idempotency (The Duplicate Guard)
   To satisfy FR-3, we implement a SHA-256 hashing mechanism.
   • Logic: Before starting "Heavy Processing", Function #1 checks if a hash of the file exists in the documentStatus container.
   • Impact: This prevents double-billing and redundant compute cycles if a user clicks "Upload" twice.
   2.3 Failure Handling (Dead-Letter Queues)
   To satisfy NFR-5, we leverage the built-in DLQ features of Service Bus.
   • Flow: If a message fails 5 times, it is automatically moved to the DLQ.
   • Observability: A dedicated DlqHandler function logs the failure to a specific Cosmos DB container for manual audit, ensuring zero data loss.
