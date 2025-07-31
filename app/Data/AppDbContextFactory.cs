using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace app.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use a default connection string for design-time operations
        // This will be overridden at runtime with the actual connection string
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SampleStoreDb;Trusted_Connection=true;MultipleActiveResultSets=true");
        
        return new AppDbContext(optionsBuilder.Options);
    }
}