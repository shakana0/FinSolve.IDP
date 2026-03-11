🚀 CI/CD Pipeline Architecture (DevOps)
This document describes the automated lifecycle of the FinSolve IDP, from code push to cloud execution.

1. The Consolidated Workflow Strategy
   To ensure system stability, we use a unified GitHub Action (deploy-main.yml) that manages dependencies between infrastructure and code.
   Stage 1: Infrastructure Deployment (deploy-infra)
   • Purpose: Provisions the 7-layer Azure environment using Bicep.
   • Senior Rationale: By running this first, we guarantee that all Resource Keys, Service Bus Namespaces, and Cosmos Containers exist before the Function App tries to connect to them.
   • Tooling: Uses azure/arm-deploy@v2 with scoped resource group deployment.
   Stage 2: Application Deployment (deploy-functions)
   • Dependency: This stage needs deploy-infra to complete successfully before starting.
   • Process: 1. Build & Publish: Compiles the .NET 10 (Isolated) project. 2. Artifact Creation: Packages the binaries for Azure Functions. 3. Deployment: Uses azure/functions-action@v1 to push the code to the pre-provisioned Function App.
2. Security & Authentication
   • OIDC (OpenID Connect): We have moved away from static AZURE_CREDENTIALS (secrets). The pipeline uses GitHub's OIDC provider to request temporary tokens from Azure.
   • Permissions: The workflow operates with id-token: write and contents: read, following the principle of least privilege.
3. Trigger Logic
   • Branch Protection: The pipeline triggers on every push and PR to the main branch.
   • Path Filtering: To optimize execution time, the workflow only triggers if changes are detected in the deployment or function directories.
