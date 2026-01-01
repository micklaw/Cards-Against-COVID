# GitHub Actions Workflows

This directory contains GitHub Actions workflows for continuous integration and deployment.

## Workflows

### PR Build (`pr-build.yml`)

**Trigger**: Pull requests to `main`, `master`, or `develop` branches

**Purpose**: Validates that code changes build successfully before merging.

**Steps**:
1. Checkout code
2. Setup .NET 8.0 SDK
3. Restore dependencies
4. Build solution in Release configuration
5. Run tests (continues on error if no tests exist)
6. Publish API and UI projects

**Usage**: Automatically runs on every pull request. No manual intervention required.

---

### Deploy to Azure (`release.yml`)

**Trigger**: 
- Push to `main` or `master` branch
- Manual workflow dispatch with environment selection

**Purpose**: Deploys the application infrastructure and code to Azure.

**Jobs**:

1. **Build**
   - Builds the solution
   - Publishes API and UI artifacts
   - Uploads artifacts for deployment jobs

2. **Deploy Infrastructure**
   - Creates Azure resource group
   - Deploys Bicep template
   - Outputs infrastructure details for subsequent jobs
   - Resources deployed:
     - Azure Static Web App
     - Azure Functions (isolated backend)
     - Azure Storage Account
     - Azure SignalR Service
     - Application Insights
     - Log Analytics Workspace

3. **Deploy Function App**
   - Downloads API artifact
   - Deploys to Azure Functions

4. **Deploy Static Web App**
   - Downloads UI artifact  
   - Deploys to Azure Static Web App
   - Displays deployment summary

**Prerequisites**:

Before running this workflow, you must:

1. **Create Azure Service Principal**:
   ```bash
   az ad sp create-for-rbac --name "github-cards-against-covid" \
     --role contributor \
     --scopes /subscriptions/{subscription-id} \
     --sdk-auth
   ```

2. **Add GitHub Secret**:
   - Go to repository Settings → Secrets and variables → Actions
   - Add new repository secret: `AZURE_CREDENTIALS`
   - Paste the JSON output from the service principal creation

3. **Verify Environment Variables**:
   - `AZURE_RESOURCE_GROUP`: Resource group name (default: `rg-cards-against-covid`)
   - `AZURE_REGION`: Azure region (default: `eastus`)

**Manual Deployment**:

To manually trigger a deployment:

1. Go to Actions tab in GitHub
2. Select "Deploy to Azure" workflow
3. Click "Run workflow"
4. Choose environment (`dev` or `prod`)
5. Click "Run workflow" button

**Environment Variables**:

You can customize the deployment by modifying these environment variables in `release.yml`:

- `DOTNET_VERSION`: .NET SDK version (default: `8.0.x`)
- `AZURE_RESOURCE_GROUP`: Target resource group
- `AZURE_REGION`: Azure deployment region

**Deployment Flow**:

```
Push to main/master
    ↓
Build Application
    ↓
Upload Artifacts
    ↓
Deploy Infrastructure (Bicep)
    ↓
    ├─→ Deploy Functions App
    └─→ Deploy Static Web App
    ↓
Deployment Complete
```

## Security

- Secrets are stored in GitHub Secrets and never exposed in logs
- Azure credentials use service principal with least-privilege access
- All connections use HTTPS with minimum TLS 1.2
- No authentication required for the application (as per requirements)

## Troubleshooting

### Build Failures

- Check that all projects target compatible .NET versions
- Verify NuGet package compatibility
- Review build logs for specific errors

### Deployment Failures

- Verify `AZURE_CREDENTIALS` secret is correctly configured
- Ensure service principal has necessary permissions
- Check Azure resource quotas and limits
- Verify resource group exists or can be created

### Infrastructure Deployment Issues

- Validate Bicep template syntax: `az bicep build --file infrastructure/main.bicep`
- Check parameter values in `infrastructure/parameters.json`
- Ensure unique resource names (handled by `uniqueString()` in Bicep)

## Monitoring

After deployment, monitor your application:

- **Application Insights**: View in Azure Portal for telemetry and errors
- **GitHub Actions**: Check workflow run history for deployment status
- **Azure Portal**: Monitor resource health and metrics
