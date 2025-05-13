using Inventory.Application.Behaviors;
using Inventory.Domain.Items;
using Inventory.Domain.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAplication(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            });

            services.AddSingleton<ITransactionFactory, TransactionFactory>();
            services.AddSingleton<IItemFactory, ItemFactory>();

            return services;
        }

    }
}
