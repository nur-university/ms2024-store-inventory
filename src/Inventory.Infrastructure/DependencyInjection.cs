using Inventory.Infrastructure.Extensions;
using Joseco.CommunicationExternal.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nur.Store2025.Security;
using System.Reflection;

namespace Inventory.Infrastructure;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );

        services.AddSecrets(configuration, environment)
            .AddPersistence()
            .AddSecurity(environment)
            .AddRabbitMQ()
            .AddObservability(environment);


        return services;
    }

    

    

    

}
