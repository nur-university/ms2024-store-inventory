using Inventory.Application.Abstractions;
using Inventory.Infrastructure.Observability;
using Inventory.Infrastructure.Persistence;
using Joseco.CommunicationExternal.RabbitMQ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inventory.Infrastructure.Extensions;

public static class ObservabilityExtensions
{

    public static IServiceCollection AddObservability(this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();

        if (environment is IWebHostEnvironment)
        {
            services.AddServicesHealthChecks();
        }

        return services;
    }

    private static IServiceCollection AddServicesHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.BuildServiceProvider().GetRequiredService<DataBaseSettings>();
        string? dbConnectionString = databaseSettings.ConnectionString;

        services
            .AddHealthChecks()
            .AddMySql(dbConnectionString)
            .AddRabbitMqHealthCheck();

        return services;
    }
}
