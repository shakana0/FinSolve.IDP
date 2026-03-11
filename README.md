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

---

## 📈 System Performance & FinOps Impact

_How the system transforms manual labor into a high-efficiency, low-cost pipeline._

| Metric                | Manual Processing | FinSolve IDP        | Improvement       |
| :-------------------- | :---------------- | :------------------ | :---------------- |
| **Time per Document** | ~5-10 mins        | **< 10 seconds**    | **98% reduction** |
| **Cost per 1k Docs**  | $$$ (Labor)       | **~1.30 SEK**       | **Fractional**    |
| **Availability**      | Working hours     | **24/7 Serverless** | **Infinite**      |

---

## 🏗️ The Event-Driven Pipeline (Visualized)

> [!TIP]
> **Architecture Walkthrough:** Below is the high-level flow of the FinSolve IDP. By moving away from traditional polling and monolithic processing, we achieve a system that is both resilient to spikes and extremely cost-efficient.


flowchart LR
Client[User/Client] -- Upload --> Blob[Azure Blob Storage]
Blob -- Event --> EG[Azure Event Grid]
EG -- Trigger --> F1[Function #1: Metadata & Validation]
F1 -- Queue Message --> SB[Azure Service Bus]
SB -- Trigger --> F2[Function #2: Heavy Document Processing]
F2 -- Persistence --> DB[(Cosmos DB / SQL)]
F2 -- HTTP Callback --> LA[Logic App: Notification]
LA -- Alert --> Email[Email/Teams]

    subgraph Reliability
        SB -- Failure --> DLQ[Dead-Letter Queue]
        DLQ -- Trigger --> F5[Function #5: DLQ Handler]
    end
    ´
---

## 🎯 Project Governance & Strategy

_As a Senior Architect, I believe that high-quality systems start with rigorous planning. This project is documented to reflect a professional Enterprise environment._

| Pillar           | Key Focus                                                 | Documentation                                                              |
| :--------------- | :-------------------------------------------------------- | :------------------------------------------------------------------------- |
| **Requirements** | FR/NFR, Scalability (10k docs), Success Metrics.          | [**Requirements Spec**](docs/REQUIREMENTS.md)                              |
| **FinOps**       | $0 Idle cost, Serverless-First, Consumption Tiers.        | [**Cost Strategy**](docs/STRATEGY.md#1-finops-cost-optimization-strategy)  |
| **Resiliency**   | Dead-Letter Queues, Idempotency (SHA-256), Load Leveling. | [**Risk Analysis**](docs/STRATEGY.md#2-risk-mitigation--system-resiliency) |


---

## 📊 Requirement Highlights (Enterprise Simulation)

To bridge the gap between business needs and technical execution, the project follows a strict requirement trace:

- **Functional (FR):** Automated ingestion via Event Grid, Async orchestration with Service Bus Topics, and Persistence in Cosmos DB.
- **Non-Functional (NFR):** Zero-Trust Security via **Managed Identity**, < 10s E2E latency, and 100% Infrastructure-as-Code.

> [!TIP]
> **Why this matters:** By defining **NFR-5 (Fault Tolerance)** before writing a single line of code, the system was built with Dead-Letter Queues (DLQ) from day one, ensuring no financial data is ever lost.

---

## 🧠 Architectural Deep Dive

> "As a Senior Engineer, I don't just build features; I architect solutions for scale, reliability, and cost-efficiency."

### 1. Event-Driven Orchestration

We use **Azure Service Bus** as a managed buffer between ingestion and heavy processing. This prevents "Function Overload" and ensures that spikes in traffic don't lead to dropped messages or billing spikes.

### 2. FinOps: The 44-SEK-per-month Strategy

The system is built on a **Consumption-based** model.

- **Zero Idle Cost:** We pay nothing when the system isn't running.
- **Smart Sampling:** Application Insights is tuned to 5% sampling to keep observability high and costs low.
- **Managed Identity:** Zero-trust security without the cost overhead of Key Vault per-secret retrieval.
---

🏗️ How It Works: The Event-Driven Pipeline
The FinSolve IDP is designed as a high-scale, asynchronous pipeline. By moving away from traditional polling and monolithic processing, we achieve a system that is both resilient to spikes and extremely cost-efficient.
System Flow (Visualized)

flowchart LR
Client[User/Client] -- Upload --> Blob[Azure Blob Storage]
Blob -- Event --> EG[Azure Event Grid]
EG -- Trigger --> F1[Function #1: Metadata & Validation]
F1 -- Queue Message --> SB[Azure Service Bus]
SB -- Trigger --> F2[Function #2: Heavy Document Processing]
F2 -- Persistence --> DB[(Cosmos DB / SQL)]
F2 -- HTTP Callback --> LA[Logic App: Notification]
LA -- Alert --> Email[Email/Teams]

    subgraph Reliability
        SB -- Failure --> DLQ[Dead-Letter Queue]
        DLQ -- Trigger --> F5[Function #5: DLQ Handler]
    end

> **Technical Deep Dive:** For a granular look at the idempotency logic, scaling strategies, and component breakdowns, see our **[Full Architecture Guide](docs/ARCHITECTURE.md)**.

---

## 🏗️ Infrastructure as Code (Bicep)

The entire cloud ecosystem is defined as code using **Azure Bicep**, ensuring 100% reproducible environments and eliminating "Configuration Drift."

- **Modular Design:** Decoupled modules for Networking, Compute, Messaging, and Storage.
- **Security First:** Zero-trust architecture using **Managed Identities**—no connection strings or keys are stored in the code.
- **Environment Parity:** Parameter-driven deployments for `dev`, `test`, and `prod` environments.

> **Deployment Deep Dive:** For a full breakdown of the resource graphing, dependency management, and security implementation, see our **[Infrastructure Guide](docs/INFRASTRUCTURE.md)**.

---

## 🚀 CI/CD & Automation

The project implements a robust **GitHub Actions** pipeline that synchronizes infrastructure state with application deployment.

- **Unified Deployment:** A consolidated workflow ensures that Bicep templates are validated and provisioned before the .NET application code is deployed.
- **OIDC Security:** Uses OpenID Connect (OIDC) for Azure authentication, eliminating the need for long-lived service principal secrets.
- **Environment Integrity:** Automated builds and tests ensure that only high-quality code reaches the `main` branch.

> **DevOps Deep Dive:** Explore the pipeline stages, security handshake, and deployment logic in the **[Workflow Guide](docs/WORKFLOW.md)**.

---

## 💰 FinOps & Cost Strategy

This architecture is engineered for **Zero Waste**. By utilizing a 100% consumption-based model, the infrastructure scales its cost linearly with the workload.

- **Azure Functions (Y1):** Scaled to zero when idle.
- **Cosmos DB Serverless:** No provisioned RU/s — pay only for actual operations.
- **Application Insights:** Smart sampling (5%) to balance observability and ingestion costs.

**[Read the full Strategic Deep Dive →](docs/STRATEGY.md)**

---

## 📖 Project Documentation

_Deep dives into the architectural, strategic, and operational layers of the FinSolve IDP._

| Document                                        | Description                                                             | Key Artifacts                             |
| :---------------------------------------------- | :---------------------------------------------------------------------- | :---------------------------------------- |
| [**📋 Requirements**](docs/REQUIREMENTS.md)     | Business goals, Functional (FR) and Non-Functional (NFR) specs.         | User Stories, Scalability targets.        |
| [**🏗️ Architecture**](docs/ARCHITECTURE.md)     | Technical deep dive into the event-driven pipeline and component roles. | Mermaid diagrams, Idempotency logic.      |
| [**🛠️ Infrastructure**](docs/INFRASTRUCTURE.md) | Full breakdown of the 7-layer Bicep modular system.                     | IaC, RBAC, Managed Identities.            |
| [**🎯 Strategy & FinOps**](docs/STRATEGY.md)    | Cost optimization, Risk mitigation, and Architectural decisions.        | Consumption-tiers, DLQ, Load Leveling.    |
| [**🚀 Workflow**](docs/WORKFLOW.md)             | DevOps lifecycle and CI/CD pipeline configuration.                      | GitHub Actions, OIDC, Environment parity. |
```
