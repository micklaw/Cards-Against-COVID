targetScope = 'subscription'

@description('Name of the resource group to create')
param resourceGroupName string

@description('Location for the resource group and all resources')
param location string

@description('Environment name (e.g., dev, staging, prod)')
param environmentName string

@description('Base name for resources')
param baseName string = 'cah'

@description('Storage account SKU')
param storageAccountSku string = 'Standard_LRS'

@description('Function App SKU')
param functionAppSku string = 'Y1'

@description('Function App SKU Family')
param functionAppSkuFamily string = 'Y'

@description('Static Web App SKU (Free or Standard)')
param staticWebAppSku string = 'Free'

// Create the resource group
resource resourceGroup 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
  tags: {
    Environment: environmentName
    Project: 'Cards-Against-COVID'
    ManagedBy: 'Bicep'
  }
}

// Deploy the main infrastructure as a module
module infrastructure 'main.bicep' = {
  name: 'infrastructure-deployment'
  scope: resourceGroup
  params: {
    location: location
    environmentName: environmentName
    baseName: baseName
    storageAccountSku: storageAccountSku
    functionAppSku: functionAppSku
    functionAppSkuFamily: functionAppSkuFamily
    staticWebAppSku: staticWebAppSku
  }
}

// Outputs from the infrastructure module
output resourceGroupName string = resourceGroup.name
output storageAccountName string = infrastructure.outputs.storageAccountName
output storageAccountConnectionString string = infrastructure.outputs.storageAccountConnectionString
output functionAppName string = infrastructure.outputs.functionAppName
output functionAppUrl string = infrastructure.outputs.functionAppUrl
output staticWebAppName string = infrastructure.outputs.staticWebAppName
output staticWebAppUrl string = infrastructure.outputs.staticWebAppUrl
output staticWebAppDeploymentToken string = infrastructure.outputs.staticWebAppDeploymentToken
