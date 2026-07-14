using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ClinicFlow.Api.Data;

public sealed class ClinicFlowDbContextFactory
    : IDesignTimeDbContextFactory<ClinicFlowDbContext>
{
    public ClinicFlowDbContext CreateDbContext(
        string[] args
    )
    {
        var environment =
            Environment.GetEnvironmentVariable(
                "ASPNETCORE_ENVIRONMENT"
            )
            ?? "Development";

        var basePath = ResolveBasePath();

        var configuration =
            new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(
                    "appsettings.json",
                    optional: false
                )
                .AddJsonFile(
                    $"appsettings.{environment}.json",
                    optional: true
                )
                .AddUserSecrets<
                    ClinicFlowDbContextFactory
                >(
                    optional: true
                )
                .AddEnvironmentVariables()
                .Build();

        var connectionString =
            configuration.GetConnectionString(
                "DefaultConnection"
            )
            ?? throw new InvalidOperationException(
                "A connection string 'DefaultConnection' não foi configurada para as migrations."
            );

        var optionsBuilder =
            new DbContextOptionsBuilder<
                ClinicFlowDbContext
            >();

        optionsBuilder.UseNpgsql(
            connectionString
        );

        return new ClinicFlowDbContext(
            optionsBuilder.Options
        );
    }

    private static string ResolveBasePath()
    {
        var currentDirectory =
            Directory.GetCurrentDirectory();

        var localAppSettings =
            Path.Combine(
                currentDirectory,
                "appsettings.json"
            );

        if (File.Exists(localAppSettings))
        {
            return currentDirectory;
        }

        var apiDirectory =
            Path.Combine(
                currentDirectory,
                "apps",
                "api"
            );

        var apiAppSettings =
            Path.Combine(
                apiDirectory,
                "appsettings.json"
            );

        if (File.Exists(apiAppSettings))
        {
            return apiDirectory;
        }

        throw new DirectoryNotFoundException(
            "Não foi possível localizar a pasta da API e o arquivo appsettings.json."
        );
    }
}