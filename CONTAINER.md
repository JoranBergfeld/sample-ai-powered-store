## Container Deployment

This application can be deployed as a Docker container using GitHub Actions and Azure Container Apps.

### Building and Testing the Container Locally

To build and test the container locally:

1. Make sure you have Docker Desktop installed and running
   - Download from [Docker Desktop](https://www.docker.com/products/docker-desktop)
   - Start Docker Desktop and ensure the Docker engine is running

2. Build the container image:
   ```powershell
   cd c:\path\to\samplestore-search-agent
   docker build -t samplestore-search-agent:local .
   ```

3. Run the container locally:
   ```powershell
   docker run -p 8080:80 samplestore-search-agent:local
   ```

4. Open a browser and navigate to `http://localhost:8080` to verify the application is running

> **Note**: If you receive an error like `error during connect: ... The system cannot find the file specified`, ensure Docker Desktop is installed and running properly.

### GitHub Actions CI/CD

The repository includes a GitHub Actions workflow that:

1. Builds the application
2. Creates a Docker container image
3. Publishes the image to GitHub Packages (as a public package)
4. Deploys the container to Azure Container Apps

To set up the GitHub Actions workflow:

1. Fork or push this repository to your GitHub account

2. Configure the following secrets in your GitHub repository:
   - `AZURE_CREDENTIALS`: Azure service principal credentials (JSON)

3. The workflow will trigger automatically on pushes to the main branch, or you can trigger it manually from the Actions tab

> **Note**: The workflow is configured to make the container image publicly accessible, so no credentials are needed to pull it.

### Manually Deploying to Azure Container Apps

To deploy the container manually to Azure Container Apps:

1. Login to Azure:
   ```powershell
   az login
   ```

2. Deploy using the enhanced deploy.ps1 script with container support:
   ```powershell
   ./deploy.ps1 -container
   ```

3. Alternatively, deploy using the Azure Developer CLI:
   ```powershell
   azd deploy
   ```

4. Or use the Bicep templates directly:
   ```powershell
   az deployment sub create --location <your-location> --template-file infra/main.bicep --parameters environmentName=<env-name> containerImage=ghcr.io/<your-username>/samplestore-search-agent:latest
   ```

For detailed Azure deployment instructions, see [Azure Deployment Guide](AZURE_DEPLOYMENT.md).

### Configuring Azure Container Apps

The Bicep templates in this repository configure Azure Container Apps to pull the container image from GitHub Packages. The configuration includes:

- Registry authentication using GitHub credentials
- Container image specification
- Environment configuration
- Scaling rules

To update the Container Apps configuration, modify the `infra/modules/containerapp.bicep` file.
