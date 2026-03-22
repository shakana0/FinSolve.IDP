<p align="center">
  <h1 align="center">FinSolve IDP</h1>
  <p align="center">
    <strong>Enterprise-Grade Serverless Intelligent Document Processing</strong>
  </p>
  <p align="center">
    Automating financial workflows with a cost-aware, event-driven architecture on Azure.
  </p>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Azure-Cloud--Native-0089D6?logo=microsoftazure" alt="Azure">
  <img src="https://img.shields.io/badge/Infrastructure-Bicep%20(IaC)-B15DA1" alt="Bicep">
  <img src="https://img.shields.io/badge/Architecture-Event--Driven-orange" alt="Event-Driven">
  <img src="https://img.shields.io/badge/FinOps-Optimized-brightgreen" alt="FinOps">
</p>

<p align="center">
  <a href="docs/REQUIREMENTS.md">📋 Requirements</a> | 
  <a href="docs/ARCHITECTURE.md">🏗️ Architecture</a> | 
  <a href="docs/INFRASTRUCTURE.md">🛠️ Infrastructure</a> | 
  <a href="docs/WORKFLOW.md">🚀 Workflow</a> | 
  <a href="docs/STRATEGY.md">🎯 Strategy</a>
</p>

---

## 📈 System Performance & FinOps Impact
_How the system transforms manual labor into a high-efficiency, low-cost pipeline._

| Metric | Manual Processing | FinSolve IDP | Improvement |
| :--- | :--- | :--- | :--- |
| **Time per Document** | ~5-10 mins | **< 10 seconds** | **98% reduction** |
| **Cost per 1k Docs** | $$$ (Labor) | **~1.30 SEK** | **Fractional** |
| **Availability** | Working hours | **24/7 Serverless** | **Infinite** |

---

## 🏗️ The Event-Driven Pipeline (Visualized)
> [!TIP]
> **Architecture Walkthrough:** Below is the high-level flow of the FinSolve IDP. By moving away from traditional polling and monolithic processing, we achieve a system that is both resilient to spikes and extremely cost-efficient.

<p align="center">
  <img src="./assets/imgs/Decision Path Selection Flow-2026-01-15-124635.png" width="800" alt="Architecture of the event-driven pipeline" />
</p>

---

## 🧠 Architectural Core Principles
> "As a Senior Engineer, I don't just build features; I architect solutions for scale, reliability, and cost-efficiency."

### 1. Event-Driven Orchestration
We use **Azure Service Bus** as a managed buffer between ingestion and heavy processing. This prevents "Function Overload" and ensures that spikes in traffic don't lead to dropped messages or billing spikes.

### 2. Zero-Trust Infrastructure (Bicep)
The entire ecosystem is defined via **Azure Bicep**, ensuring 100% reproducible environments.
- **Identity-Driven:** Uses **Managed Identities**—no connection strings or keys are stored in the code.
- **Modular Design:** Decoupled modules for Networking, Compute, Messaging, and Storage.

### 3. CI/CD & Automation
A robust **GitHub Actions** pipeline synchronizes infrastructure state with application deployment.
- **OIDC Security:** Uses OpenID Connect for Azure authentication, eliminating long-lived secrets.
- **Environment Integrity:** Automated builds ensure only high-quality code reaches `main`.

---

## 🏁 Getting Started

### Prerequisites
* **Azure CLI** & **Bicep CLI**
* **.NET 10 SDK** (Isolated Worker)
* **Azure Functions Core Tools** (for local debugging)

### 1. Infrastructure Deployment
Deploy the foundational Azure resources using the provided Bicep templates:
```bash
az deployment group create \
  --resource-group <your-rg-name> \
  --template-file ./deployment/main.bicep \
  --parameters prefix=finsolve env=dev
```

### 2. Local Configuration
Create a `local.settings.json` in the function directory:

```diff
 {
   "IsEncrypted": false,
   "Values": {
     "AzureWebJobsStorage": "UseDevelopmentStorage=true",
     "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
+    "ServiceBusConnection__fullyQualifiedNamespace": "<your-bus>.servicebus.windows.net",
+    "CosmosDbAccountEndpoint": "<your-cosmos-endpoint>",
+    "KeyVaultUri": "https://<your-vault>.vault.azure.net/"
   }
 }
```

### 3. Run the Application
```bash
cd FinSolve.IDP.Application
func start
```
---

## 🎯 Project Governance & Strategy
_This project is documented to reflect a professional Enterprise environment._

| Pillar | Key Focus | Documentation |
| :--- | :--- | :--- |
| **Requirements** | FR/NFR, Scalability (10k docs), Success Metrics. | [**Requirements Spec**](docs/REQUIREMENTS.md) |
| **Architecture** | Component roles, Idempotency, and Logic flow. | [**Technical Deep Dive**](docs/ARCHITECTURE.md) |
| **Infrastructure** | Modular Bicep, RBAC, and Resource Layers. | [**Infrastructure Spec**](docs/INFRASTRUCTURE.md) |
| **DevOps** | GitHub Actions, OIDC, and Environment Parity. | [**Workflow Guide**](docs/WORKFLOW.md) |
| **FinOps** | $0 Idle cost, Serverless-First, Consumption Tiers. | [**Cost Strategy**](docs/STRATEGY.md) |

---

## 💰 FinOps: The Consumption-First Strategy
This architecture is engineered for **Zero Waste**. By utilizing a 100% consumption-based model, the infrastructure cost scales linearly with the workload.

- **Azure Functions (Y1):** Scaled to zero when idle.
- **Cosmos DB Serverless:** No provisioned RU/s — pay only for actual operations.
- **App Insights:** Smart sampling (5%) to balance observability and ingestion costs.

> [!NOTE]
> **Why this matters:** By defining **NFR-5 (Fault Tolerance)** before writing code, the system was built with Dead-Letter Queues (DLQ) from day one, ensuring no financial data is ever lost.

---
