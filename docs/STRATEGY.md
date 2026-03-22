---

# 🏦 FinSolve IDP: Intelligent Document Processing

## 📋 Requirement Specification
_This section defines the functional and non-functional requirements, ensuring strict alignment between business goals and technical execution._

### 1. Business Context & Goal
The manual handling of financial documents is slow, error-prone, and expensive. The goal of **FinSolve IDP** is to provide a 100% automated, cloud-native pipeline that extracts metadata and insights from uploaded documents with minimal human intervention.

#### **Success Metrics:**
* **Efficiency:** Reduce document processing time from minutes to seconds (**< 10s**).
* **Cost:** Maintain an operational cost of less than **2 SEK per 1,000 documents**.
* **Accuracy:** Ensure **100% data integrity** through validation and idempotency checks.

### 2. Functional Requirements (FR)
| ID | Requirement | Description |
| :--- | :--- | :--- |
| **FR-1** | **Automated Ingestion** | Trigger a processing workflow automatically upon file upload to Azure Blob Storage. |
| **FR-2** | **Metadata Validation** | Validate file type (PDF/TXT/JSON/CSV/DOCX) and size before further processing. |
| **FR-3** | **Idempotent Processing** | Detect duplicate uploads using SHA-256 hashing to prevent redundant compute costs. |
| **FR-4** | **Async Orchestration** | Decouple processing steps via a message broker (Service Bus) for independent scaling. |
| **FR-5** | **Persistence** | Store extracted data and document status in a schema-less database (Cosmos DB). |
| **FR-6** | **External Notification** | Notify stakeholders via Logic Apps upon successful completion or critical failure. |

---

## 🏗️ Technical Architecture Deep Dive
_A granular analysis of the architectural decisions and the "Senior Rationale" behind the pipeline._

> [!TIP]
> **Architecture Walkthrough:** Below is the high-level flow. By moving away from traditional polling and monolithic processing, we achieve a system that is both resilient to spikes and extremely cost-efficient.

### 1. The Ingestion Layer
* **Azure Event Grid (The Push Model):** Monitors Blob Storage and routes `BlobCreated` events. 
    * *Senior Rationale:* Avoids the standard BlobTrigger (polling) to eliminate idle execution costs.
* **Function #1 (Metadata & Validation):** Validates constraints and calculates a SHA-256 hash.
    * *Senior Rationale:* Early hashing implements **Idempotency**, terminating redundant processes before expensive "Heavy Processing" begins.

### 2. The Orchestration & Messaging Layer
* **Azure Service Bus (The Load Leveler):** Acts as a persistent buffer.
    * *Senior Rationale:* Prevents "Function Overload" during traffic spikes, allowing downstream functions to process at a sustainable rate.

### 3. The Processing & Persistence Layer
* **Function #2 (Heavy Processing):** Performs business logic, data extraction, and mapping.
    * *Senior Rationale:* Isolated and stateless, allowing extraction logic to be updated without affecting ingestion speed.
* **Cosmos DB (Serverless):** Stores results and metadata.
    * *Senior Rationale:* Aligns perfectly with the "Pay-per-use" philosophy.

---

## 🛠️ Infrastructure as Code Specification
_Modular Bicep architecture used to provision the 7-layer ecosystem._

### 1. Compute Layer (`functionapp.bicep`)
* **Resource Plan:** Utilizes the **Y1 Dynamic tier** (Serverless).
* **Identity-Driven Security:** Uses **System-Assigned Managed Identity** to eliminate connection strings.
* **Runtime:** Built for **.NET Isolated**, separating the function host from application code.

### 2. Messaging Layer (`servicebus.bicep`)
* **Topic-Based Routing:** Uses a single topic (`idp-documents`) with **SQL Filters** for fine-grained routing.
* **Resiliency:** Configured with `maxDeliveryCount: 5` and Dead-Lettering to prevent data loss.

### 3. Observability Layer (`appinsights.bicep`)
* **Telemetry:** Provides distributed tracing and acts as the primary data source for **FinOps cost-allocation**.

---

## 🚀 CI/CD & Automation Workflow
_A robust DevOps lifecycle synchronizing infrastructure state with application deployment._

### The Consolidated Workflow Strategy
We use a unified GitHub Action (`deploy-main.yml`) to manage dependencies.

1.  **Stage 1: Infrastructure (deploy-infra):** Provisions the environment using **Bicep**.
2.  **Stage 2: Application (deploy-functions):** Compiles .NET 10 code and performs a Zip Deploy to the pre-provisioned Function App.

> [!IMPORTANT]
> **Security (OIDC):** The pipeline leverages **OpenID Connect** to request temporary tokens from Azure, moving away from static secrets.

---

## 🎯 Architectural Strategy & FinOps
_Bridging the gap between requirements and implementation with a focus on cloud economy._

### FinOps: Cost Optimization Strategy
| Service | SKU / Tier | Optimization Strategy |
| :--- | :--- | :--- |
| **Azure Functions** | Consumption (Y1) | **Zero Idle Cost:** Pay only for milliseconds executed. Scales to zero. |
| **Cosmos DB** | Serverless | **No Provisioned Throughput:** Eliminates costs for unused RU/s. |
| **Service Bus** | Standard | **Enterprise Pub/Sub:** Enables Topics and SQL-filters at a fixed low cost. |
| **App Insights** | Log Analytics | **Smart Sampling:** Configured at 5% to minimize ingestion costs while maintaining traceability. |

### Risk Mitigation & Resiliency
* **Load Leveling:** Service Bus buffers spikes of up to 10,000+ documents.
* **Failure Handling (DLQ):** If a message fails 5 times, it is moved to the **Dead-Letter Queue**. A dedicated `DlqHandler` logs these to Cosmos DB for manual audit.

---
