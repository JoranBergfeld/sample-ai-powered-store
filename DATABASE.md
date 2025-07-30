# Database Configuration

The application now supports both in-memory database (for development) and Azure SQL Database (for production deployment).

## Local Development

When running locally without a database connection string, the application automatically uses an in-memory database with seeded sample data.

```bash
dotnet run
```

## Azure Deployment

The Azure infrastructure now includes an Azure SQL Database that is automatically provisioned when deploying with `azd`.

### Prerequisites

Before deploying, you need to set the SQL administrator password:

```bash
# Set the SQL administrator password as an environment variable
export SQL_ADMINISTRATOR_PASSWORD="YourSecurePassword123!"

# Or set it using azd env
azd env set SQL_ADMINISTRATOR_PASSWORD "YourSecurePassword123!"
```

### Deployment

Deploy the application with the database:

```bash
azd up
```

The deployment will:
1. Create an Azure SQL Server with the specified administrator credentials
2. Create a SQL Database named "samplestore"
3. Configure the Container App with the database connection string
4. Run Entity Framework migrations to create tables and seed data

### Database Configuration

The SQL Database is configured with:
- **Server**: `sql-<environment-hash>` (automatically generated)
- **Database**: `samplestore`
- **Administrator Login**: `dbadmin` (default, can be customized in main.bicep)
- **SKU**: Basic (suitable for development/testing)
- **Max Size**: 1 GB

### Connection String

The application receives the connection string via environment variable:
```
ConnectionStrings__DefaultConnection=Server=tcp:sql-xyz.database.windows.net,1433;Initial Catalog=samplestore;Persist Security Info=False;User ID=dbadmin;Password=***;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### Database Migrations

The application automatically applies Entity Framework migrations on startup when using SQL Server. If you need to add new migrations:

```bash
# Add a new migration
dotnet ef migrations add <MigrationName>

# Remove the last migration (if not yet applied)
dotnet ef migrations remove
```

### Security

- The database is configured to allow Azure services access
- All connections use SSL encryption
- The administrator password is securely managed through Azure deployment parameters
- Connection strings are injected as environment variables in the Container App

### Monitoring

You can monitor the database through:
- Azure Portal > SQL Database metrics
- Application logs for Entity Framework operations
- Azure Monitor and Application Insights (if configured)