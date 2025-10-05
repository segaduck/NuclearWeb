using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NuclearWeb.Infrastructure.Data;

/// <summary>
/// Design-time factory for ApplicationDbContext
/// Used by EF Core tools for migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use a default connection string for migrations
        // This will be overridden at runtime by appsettings.json
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=nuclearweb;User=root;Password=password;",
            new MySqlServerVersion(new Version(8, 0, 21))
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
