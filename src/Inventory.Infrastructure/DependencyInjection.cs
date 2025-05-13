using Inventory.Application;
using Inventory.Domain.Items;
using Inventory.Domain.Transactions;
using Inventory.Domain.Users;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence.DomainModel;
using Inventory.Infrastructure.Persistence.Repositories;
using Inventory.Infrastructure.Persistence.StoredModel;
using Inventory.Infrastructure.RabbitMQ.Consumers;
using Joseco.Communication.External.RabbitMQ.Services;
using Joseco.CommunicationExternal.RabbitMQ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Joseco.DDD.Core.Abstractions;
using Nur.Store2025.Integration.Catalog;
using Nur.Store2025.Integration.Identity;
using System.Reflection;
using Nur.Store2025.Security;
using Nur.Store2025.Security.Config;
using Joseco.Secrets.HashicorpVault;
using Joseco.Secrets.Contrats;
using System;
using Nur.Store2025.Access.Inventory.Permissions;
using Inventory.Application.Abstractions;
using Inventory.Infrastructure.Observability;

namespace Inventory.Infrastructure;

public static class DependencyInjection
{
    private const string JwtOptionsSecretName = "JwtOptions";
    private const string RabbitMqSettingsSecretName = "RabbitMqSettings";
    private const string InventoryDatabaseConnectionStringSecretName = "InventoryDatabaseConnectionString";

    private const string VaultMountPoint = "secrets";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );

        services.AddVault()
            .AddPersistence()
            .AddSecurity(environment)
            .AddRabbitMQ()
            .AddObservability();

        if(environment is IWebHostEnvironment)
        {
            services.AddServicesHealthChecks();
        }

        return services;
    }

    private static IServiceCollection AddVault(this IServiceCollection services)
    {
        string? vaultUrl = Environment.GetEnvironmentVariable("VAULT_URL");
        string? vaultToken = Environment.GetEnvironmentVariable("VAULT_TOKEN");

        if (string.IsNullOrEmpty(vaultUrl) || string.IsNullOrEmpty(vaultToken))
        {
            throw new InvalidOperationException("Vault URL or Token is not set in environment variables.");
        }

        var settings = new VaultSettings()
        {
            VaultUrl = vaultUrl,
            VaultToken = vaultToken
        };

        services.AddHashicorpVault(settings)
            .LoadSecretsFromVault();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        var databaseSettings = services.BuildServiceProvider().GetRequiredService<DataBaseSettings>();
        var dbConnectionString = databaseSettings.ConnectionString;

        services.AddDbContext<StoredDbContext>(context =>
                context.UseMySql(dbConnectionString,
                    ServerVersion.AutoDetect(dbConnectionString)));
        services.AddDbContext<DomainDbContext>(context =>
                context.UseMySql(dbConnectionString,
                    ServerVersion.AutoDetect(dbConnectionString)));

        services.AddScoped<IDatabase, StoredDbContext>();

        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddSecurity(this IServiceCollection services, IHostEnvironment environment)
    {
        if (environment is IWebHostEnvironment)
        {
            var jwtOptions = services.BuildServiceProvider().GetRequiredService<JwtOptions>();
            services.AddSecurity(jwtOptions, InventoryPermission.PermissionsList);
        }

        return services;
    }

    private static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var rabbitMqSettings = serviceProvider.GetRequiredService<RabbitMqSettings>();

        services.AddRabbitMQ(rabbitMqSettings)
            .AddRabbitMqConsumer<ProductCreated, ProductCreatedConsumer>("inventory-product-created")
            .AddRabbitMqConsumer<UserCreated, UserCreatedConsumer>("inventory-user-created");

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

    private static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
        return services;
    }

    private static void LoadSecretsFromVault(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        using var scope = scopeFactory.CreateScope();
        var secretManager = scope.ServiceProvider.GetRequiredService<ISecretManager>();

        Task[] tasks = [
                LoadAndRegister<JwtOptions>(secretManager, services, JwtOptionsSecretName, VaultMountPoint),
                LoadAndRegister<RabbitMqSettings>(secretManager, services, RabbitMqSettingsSecretName, VaultMountPoint),
                LoadAndRegister<DataBaseSettings>(secretManager, services, InventoryDatabaseConnectionStringSecretName, VaultMountPoint)
            ];


        Task.WaitAll(tasks);
    }

    private static async Task LoadAndRegister<T>(ISecretManager secretManager, IServiceCollection services,
        string secretName, string mountPoint) where T : class, new()
    {
        T secret = await secretManager.Get<T>(secretName, mountPoint);
        services.AddSingleton<T>(secret);
    }
}
