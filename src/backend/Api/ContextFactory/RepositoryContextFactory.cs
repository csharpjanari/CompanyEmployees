using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace CompanyEmployees.ContextFactory;

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var mySqlConnection = configuration.GetConnectionString("mySqlConnection");

        var builder = new DbContextOptionsBuilder<RepositoryContext>()
            .UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection),
                b => b.MigrationsAssembly("Api"));

        return new RepositoryContext(builder.Options);
    }
}