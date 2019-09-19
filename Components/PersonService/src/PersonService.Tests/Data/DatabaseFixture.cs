using System;
using System.Collections.Generic;

using MainProject.Common.Data;
using MainProject.Common.Data.Updater;
using MainProject.Common.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PersonService.Data;
using PersonService.Data.Migration;

namespace PersonService.Tests.Data
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new[]
                    {
                        new KeyValuePair<string, string>("db:address", configuration.GetValue<string>("db:address") ?? "localhost"),
                        new KeyValuePair<string, string>("db:port", configuration.GetValue<string>("db:port") ?? "5434"),
                        new KeyValuePair<string, string>("db:password", "password01"),
                        new KeyValuePair<string, string>("db:userName", "postgres"),
                        new KeyValuePair<string, string>("db:name", (configuration.GetValue<string>("db:name") ?? "person_service_") + Guid.NewGuid().ToString().Replace("-", string.Empty)),
                        new KeyValuePair<string, string>("db:journalTable", "schema_version")
                    })
                .Build();

            var serviceCollection = new ServiceCollection()
                .AddLogging()
                .AddEssentials()
                .AddDataServices(configuration);

            ServiceProvider = serviceCollection
                .BuildServiceProvider();

            ServiceProvider
                .GetService<ISchemaUpdater>()
                .UpdateSchemas<Anchor>();
        }

        ~DatabaseFixture()
        {
            Dispose(false);
        }

        public IServiceProvider ServiceProvider { get; set; }

        public void ResetDataInTestDatabase(params string[] tableNames)
        {
            var migrator = ServiceProvider.GetService<DataSchemaUpdater>();
            migrator.ResetDataInTestDatabase(tableNames);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            var migrator = ServiceProvider.GetService<DataSchemaUpdater>();
            migrator.DropDatabase();
            ServiceProvider = null;
        }
    }
}