using System;
using System.Data.Common;
using MainProject.Common.Data.Models;
using MainProject.Common.Data.Updater;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Npgsql;

namespace MainProject.Common.Data
{
    public static class Configuration
    {
        public static IServiceCollection AddCoreDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<DataOptions>(configuration.GetSection("db"))
                .AddTransient<ISchemaUpdater, SchemaUpdater>()
                .AddTransient<DataSchemaUpdater>()
                .AddSingleton<DbProviderFactory, NpgsqlFactory>(x => NpgsqlFactory.Instance)
                .AddScoped<ITransactionManager, TransactionManager>(TransactionManagerFactory);
        }

        private static TransactionManager TransactionManagerFactory(IServiceProvider services)
        {
            return new TransactionManager(
                services.GetService<DbProviderFactory>(),
                services.GetService<IOptions<DataOptions>>().Value.ConnectionString);
        }
    }
}
