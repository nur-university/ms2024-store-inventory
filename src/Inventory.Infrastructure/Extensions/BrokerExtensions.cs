using Inventory.Infrastructure.RabbitMQ.Consumers;
using Joseco.Communication.External.RabbitMQ.Services;
using Joseco.CommunicationExternal.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Nur.Store2025.Integration.Catalog;
using Nur.Store2025.Integration.Identity;

namespace Inventory.Infrastructure.Extensions;

public static class BrokerExtensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var rabbitMqSettings = serviceProvider.GetRequiredService<RabbitMqSettings>();

        services.AddRabbitMQ(rabbitMqSettings)
            .AddRabbitMqConsumer<ProductCreated, ProductCreatedConsumer>("inventory-product-created")
            .AddRabbitMqConsumer<UserCreated, UserCreatedConsumer>("inventory-user-created");

        return services;
    }
}
