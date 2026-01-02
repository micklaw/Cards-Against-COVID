# Infrastructure as Code

This directory contains Bicep templates for deploying the Cards Against COVID infrastructure to Azure.

## Resources

The infrastructure includes:

- **Azure Static Web App**: Hosts the React UI
- **Azure Functions**: Isolated (.NET 8.0) serverless backend API
- **Azure Storage Account**: Stores game state in Azure Tables

## Deployment Approach

This project supports two deployment approaches:

### 1. Subscription-Level Deployment (Recommended)

Uses `main-subscription.bicep` to deploy at subscription scope, which creates the resource group and all resources in a single deployment.

### 2. Resource Group-Level Deployment

Uses `main.bicep` to deploy resources to an existing resource group.

## Deployment

### Prerequisites

- Azure CLI installed
- Azure subscription with appropriate permissions
- Service Principal with Contributor role (for GitHub Actions)

### Manual Deployment - Subscription Level (Recommended)

```bash
# Login to Azure
az login

# Deploy infrastructure at subscription level (creates resource group + resources)
az deployment sub create \
  --location eastus \
  --template-file infrastructure/main-subscription.bicep \
  --parameters resourceGroupName=rg-cards-against-covid \
  --parameters location=eastus \
  --parameters environmentName=dev \
  --parameters baseName=cah \
  --parameters storageAccountSku=Standard_LRS \
  --parameters functionAppSku=Y1 \
  --parameters functionAppSkuFamily=Y
```

### Manual Deployment - Resource Group Level

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

The infrastructure is automatically deployed via GitHub Actions workflows:

- **`deploy-to-azure.yml`**: New configurable deployment using subscription-level Bicep
- **`release.yml`**: Existing deployment workflow for dev/staging
- **`tag-release.yml`**: Production deployment on version tags

#### Setup GitHub Secrets for deploy-to-azure.yml

Add the following secrets to your GitHub repository:

1. **AZURE_CREDENTIALS** (Required): Service Principal credentials in JSON format
   ```bash
   # First, get your subscription ID
   SUBSCRIPTION_ID=$(az account show --query id -o tsv)
   
   # Then create the service principal
   az ad sp create-for-rbac --name "github-cards-against-covid" \
     --role contributor \
     --scopes /subscriptions/$SUBSCRIPTION_ID \
     --json-auth
   ```
   
   **Note**: Replace `$SUBSCRIPTION_ID` with your actual subscription ID if running commands separately.
   
   The output should look like:
   ```json
   {
     "clientId": "<GUID>",
     "clientSecret": "<STRING>",
     "subscriptionId": "<GUID>",
     "tenantId": "<GUID>"
   }
   ```

2. **RESOURCE_GROUP_NAME** (Optional): Name of the resource group to create
   - Default: `rg-cards-against-covid`
   - Example: `rg-my-game-prod`

3. **LOCATION** (Optional): Azure region for deployment
   - Default: `eastus`
   - Example: `westus2`, `northeurope`, etc.

4. **AZURE_SUBSCRIPTION_ID** (Optional): Azure subscription ID
   - Used for validation and logging
   - Can be found in the AZURE_CREDENTIALS JSON

#### Using the deploy-to-azure.yml Workflow

The workflow can be triggered in two ways:

1. **Automatic**: Pushes to `main` or `master` branches automatically deploy to dev environment
2. **Manual**: Use the "Run workflow" button in GitHub Actions tab to deploy to a specific environment (dev, staging, or prod)

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
