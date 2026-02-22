using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ModuloNet.Infrastructure.Persistence;

/// <summary>
/// Enables design-time creation of <see cref="ApplicationDbContext"/> for EF Core migrations.
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = GetAppSettingsBasePath();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=ModuloNet;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string GetAppSettingsBasePath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            Path.Combine(currentDir, "src", "ModuloNet.Api"),   // from solution root
            currentDir,                                          // from Api project dir
            Path.Combine(currentDir, "..", "ModuloNet.Api")      // from Infrastructure or src
        };
        foreach (var path in candidates)
        {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(Path.Combine(fullPath, "appsettings.json")))
                return fullPath;
        }
        return currentDir;
    }
}
