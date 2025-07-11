## Azure Container Apps Deployment Guide

This guide provides detailed instructions for deploying the SampleStore Search Agent application to Azure Container Apps, either through manual steps or using the GitHub Actions workflow.

### Prerequisites

- An Azure subscription
- Azure CLI installed
- GitHub account with access to the repository
- Docker installed locally (for testing)

### Deployment Options

#### Option 1: Deploy using GitHub Actions (Recommended)

1. **Fork or clone** the repository to your GitHub account

2. **Configure GitHub Secrets**:
   - Go to your repository's Settings > Secrets and variables > Actions
   - Add the following secrets:
     - `AZURE_CREDENTIALS`: JSON credentials for Azure service principal (see below for how to create)

3. **Create Azure Service Principal**:
   ```powershell
   # Login to Azure
   az login

   # Create a resource group if you don't have one
   az group create --name samplestore-rg --location swedencentral

   # Create service principal and get credentials
   az ad sp create-for-rbac --name "samplestore-sp" --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/samplestore-rg --json-auth
   ```
   
   Copy the JSON output and save it as the `AZURE_CREDENTIALS` secret in GitHub.

4. **Trigger the workflow**:
   - Go to the Actions tab in your repository
   - Select the "Build and Publish Container" workflow
   - Click "Run workflow" and select the main branch

5. **Verify deployment**:
   - Once the workflow completes, go to the Azure portal
   - Find the Container App in your resource group
   - Click on the application URL to access the deployed app

#### Option 2: Manual Deployment with Azure CLI

1. **Login to Azure**:
   ```powershell
   az login
   ```

2. **Create a resource group**:
   ```powershell
   az group create --name samplestore-rg --location swedencentral
   ```

3. **Deploy using the Bicep template**:
   ```powershell
   az deployment sub create \
     --location swedencentral \
     --template-file infra/main.bicep \
     --parameters environmentName=dev \
                  containerImage=ghcr.io/yourusername/samplestore-search-agent:latest
   ```

4. **Get the application URL**:
   ```powershell
   az containerapp show -g samplestore-rg -n samplestore-search-agent --query properties.configuration.ingress.fqdn -o tsv
   ```

### Configuration Options

The Container Apps deployment can be customized by modifying:

- **Scale settings**: Edit the minReplicas and maxReplicas parameters in containerapp.bicep
- **Resource allocation**: Adjust containerCpuCoreCount and containerMemory in containerapp.bicep
- **Environment variables**: Add environment variables to pass configuration to the container

### Public GitHub Packages

This application uses public GitHub Packages for container images. Benefits include:

- **No credentials needed**: Public packages can be pulled without authentication
- **Simplified deployment**: Azure Container Apps can pull images directly without registry credentials
- **Better collaboration**: Team members and contributors can access container images easily

If you need to make your packages private, you'll need to update the Bicep templates to include registry credentials.

### Troubleshooting

1. **Container fails to start**:
   - Check container logs in the Azure portal
   - Verify that the image exists in GitHub Packages
   - Ensure the registry credentials are correct

2. **Cannot connect to deployed app**:
   - Verify the Container App is in a "Running" state
   - Check if the ingress is configured as "external"
   - Ensure that the port configuration matches the application

3. **GitHub Actions workflow fails**:
   - Check the workflow logs for detailed error messages
   - Verify that the AZURE_CREDENTIALS secret is properly formatted
   - Ensure the service principal has sufficient permissions

### Resources

- [Azure Container Apps documentation](https://docs.microsoft.com/azure/container-apps/)
- [GitHub Actions for Azure](https://github.com/Azure/actions)
- [GitHub Packages documentation](https://docs.github.com/packages)
