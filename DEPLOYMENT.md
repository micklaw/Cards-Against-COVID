# Azure Deployment Setup Guide

This guide walks you through setting up Azure deployment for the Cards Against COVID project using the tag-based deployment workflow.

## Prerequisites

- Azure subscription
- GitHub repository access with admin permissions
- Azure CLI installed locally (for service principal creation)

## Step-by-Step Setup

### 1. Create Azure Service Principal

The service principal allows GitHub Actions to authenticate with Azure and deploy resources.

```bash
# Login to Azure
az login

# Get your subscription ID (save this value)
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
echo "Your subscription ID is: $SUBSCRIPTION_ID"

# Create service principal with contributor role at subscription level
# The SUBSCRIPTION_ID from above will be automatically used
az ad sp create-for-rbac \
  --name "github-cards-against-covid" \
  --role contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID \
  --json-auth
```

**Note**: If running commands separately, replace `$SUBSCRIPTION_ID` with your actual subscription ID from the first command.

The output will look like this:
```json
{
  "clientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "clientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "subscriptionId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "tenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

**Important**: Save this output securely - you'll need it for GitHub secrets.

### 2. Configure GitHub Secrets

Navigate to your GitHub repository settings and add the following secrets:

1. Go to **Settings** → **Secrets and variables** → **Actions**
2. Click **New repository secret**
3. Add each of the following secrets:

#### Required Secrets

| Secret Name | Value | Description |
|------------|-------|-------------|
| `AZURE_CREDENTIALS` | The entire JSON output from step 1 | Service Principal credentials for Azure authentication |

#### Optional Secrets

| Secret Name | Default Value | Description |
|------------|---------------|-------------|
| `RESOURCE_GROUP_NAME` | `rg-cards-against-covid` | Name of the resource group to create |
| `LOCATION` | `eastus` | Azure region for deployment |
| `AZURE_SUBSCRIPTION_ID` | (from AZURE_CREDENTIALS) | Your Azure subscription ID |

**Note**: If you don't provide optional secrets, the default values will be used.

### 3. Trigger Deployment

Deployment is triggered automatically when you create and push a version tag:

```bash
# Create a new version tag
git tag v1.0.0

# Push the tag to trigger deployment
git push origin v1.0.0
```

The tag must follow the pattern `v*.*.*` (e.g., v1.0.0, v2.1.3) to trigger the deployment workflow.

### 4. Monitor Deployment

1. Go to **Actions** tab in GitHub
2. Click on the running workflow (triggered by your tag)
3. Monitor the progress of each job:
   - **build**: Compiles and packages the application
   - **deploy-infrastructure**: Creates Azure resources
   - **deploy-function-app**: Deploys the backend API
   - **deploy-static-web-app**: Deploys the frontend

### 5. Access Your Deployment

After successful deployment, you'll see a deployment summary with:
- Resource Group Name
- Static Web App URL (your application URL)
- Function App Name
- Environment name

## Customizing the Deployment

### Change Resource Group Name

To deploy to a different resource group:
1. Update the `RESOURCE_GROUP_NAME` secret in GitHub
2. Trigger a new deployment

### Change Azure Region

To deploy to a different region:
1. Update the `LOCATION` secret in GitHub
2. Trigger a new deployment

## Troubleshooting

### Error: "Authorization failed"

**Cause**: Service principal doesn't have sufficient permissions.

**Solution**: 
```bash
# Re-create the service principal with correct permissions
az ad sp create-for-rbac \
  --name "github-cards-against-covid" \
  --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --json-auth
```

### Error: "Resource group already exists"

**Cause**: The resource group was created manually or in a previous deployment.

**Solution**: This is normal - the Bicep template will use the existing resource group.

### Error: "Location not available"

**Cause**: The specified location doesn't support required services.

**Solution**: Choose a different location. Recommended locations:
- `eastus`
- `westus2`
- `northeurope`
- `southeastasia`

## Architecture Overview

The deployment creates the following Azure resources:

1. **Resource Group**: Container for all resources
2. **Storage Account**: Stores game state in Azure Tables
3. **Azure Functions**: Serverless backend API (.NET 8.0)
4. **App Service Plan**: Consumption plan for Functions
5. **Static Web App**: Hosts the React frontend

All resources are created with:
- HTTPS-only access
- TLS 1.2 minimum
- Secure defaults

## Cost Estimation

The infrastructure uses cost-effective Azure services:

- **Azure Functions**: Consumption plan (~$0 for low usage)
- **Storage Account**: Standard LRS (~$0.02/GB/month)
- **Static Web Apps**: Free tier
- **App Service Plan**: Consumption (pay-per-execution)

**Estimated monthly cost**: $5-20 depending on usage

## Next Steps

After deployment:
1. Test the application by accessing the Static Web App URL
2. Configure custom domain (optional)
3. Set up monitoring and alerts
4. Configure Application Insights for telemetry

## Support

For issues or questions:
- Check the workflow logs in GitHub Actions
- Review the Bicep templates in `infrastructure/` directory
- Consult Azure documentation for service-specific issues
