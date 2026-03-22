---

## 🚀 CI/CD & Automation Workflow
_A robust DevOps lifecycle that synchronizes infrastructure state with application deployment._

> [!IMPORTANT]
> **Operational Overview:** This visualization bridges the gap between development and production. It demonstrates a fully automated CI/CD pipeline using GitHub Actions and Bicep, which provisions a resilient, event-driven architecture on Azure.

### Automated Lifecycle: From Code to Cloud
![Architecture Overview](../assets/imgs/Decision%20Path%20Selection%20Flow-2026-01-17-091120.png)

---

### 1. The Consolidated Workflow Strategy
To ensure system stability, we use a unified GitHub Action (`deploy-main.yml`) that strictly manages dependencies between infrastructure and code.

#### **Stage 1: Infrastructure Deployment (deploy-infra)**
* **Purpose:** Provisions the 7-layer Azure environment using **Bicep**.
* **Senior Rationale:** By running this first, we guarantee that all Resource Keys, Service Bus Namespaces, and Cosmos Containers exist before the Function App attempts a connection.
* **Tooling:** Utilizes `azure/arm-deploy@v2` with scoped resource group deployment.

#### **Stage 2: Application Deployment (deploy-functions)**
* **Dependency:** This stage requires `deploy-infra` to complete successfully before initiation.
* **Process:**
    1.  **Build & Publish:** Compiles the .NET 10 (Isolated) project.
    2.  **Artifact Creation:** Packages binaries specifically for Azure Functions.
    3.  **Deployment:** Uses `azure/functions-action@v1` to push code to the pre-provisioned Function App.

---

### 2. Security & Authentication
* **OIDC (OpenID Connect):** We have migrated away from static `AZURE_CREDENTIALS`. The pipeline leverages GitHub's OIDC provider to request temporary, short-lived tokens from Azure.
* **Principle of Least Privilege:** The workflow operates with strictly scoped permissions (`id-token: write` and `contents: read`) to minimize the security surface area.

---

### 3. Trigger Logic & Optimization
* **Branch Protection:** The pipeline is enforced on every `push` and `Pull Request` to the `main` branch.
* **Path Filtering:** To optimize execution time and reduce compute waste, the workflow only triggers when changes are detected within the `/deployment` or `/function` directories.

> [!NOTE]
> This automation ensures 100% environment parity, eliminating "it works on my machine" issues by treating the cloud environment as a versioned artifact.
