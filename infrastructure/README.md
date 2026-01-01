# Infrastructure as Code

This directory contains Bicep templates for deploying the Cards Against COVID infrastructure to Azure.

## Resources

The infrastructure includes:

- **Azure Static Web App**: Hosts the Blazor WebAssembly UI
- **Azure Functions**: Isolated (.NET 8.0) serverless backend API
- **Azure Storage Account**: Stores game state in Azure Tables
- **Azure SignalR Service**: Real-time communication for game updates
- **Application Insights**: Application monitoring and telemetry
- **Log Analytics**: Centralized logging

## Deployment

### Prerequisites

- Azure CLI installed
- Azure subscription
- Resource group created

### Manual Deployment

```bash
# Login to Azure
az login

# Create a resource group (if not exists)
az group create --name rg-cards-against-covid --location eastus

# Deploy the infrastructure
az deployment group create \
  --resource-group rg-cards-against-covid \
  --template-file infrastructure/main.bicep \
  --parameters infrastructure/parameters.json
```

### GitHub Actions Deployment

The infrastructure is automatically deployed via GitHub Actions workflow when changes are pushed to the main branch. See `.github/workflows/release.yml` for details.

## Parameters

Edit `parameters.json` to customize the deployment:

- `environmentName`: Environment identifier (dev, staging, prod)
- `baseName`: Base name for all resources
- `storageAccountSku`: Storage account SKU (Standard_LRS, Standard_GRS, etc.)
- `functionAppSku`: Function App SKU (Y1 for consumption)
- `functionAppSkuFamily`: Function App SKU family (Y for consumption)

## Security

- All resources use HTTPS only
- Minimum TLS version is 1.2
- Storage account has public blob access disabled
- Function App uses managed identity where possible
- No authentication required (as specified in requirements)
