using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace MainProject.Common.Data
{
    public class SchemaUpdateService<T> : BackgroundService
    {
        private readonly IApplicationLifetime _applicationLifetime;

        private readonly ISchemaUpdater _schemaUpdater;

        public SchemaUpdateService(IApplicationLifetime applicationLifetime, ISchemaUpdater schemaUpdater)
        {
            _applicationLifetime = applicationLifetime;
            _schemaUpdater = schemaUpdater;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _schemaUpdater.UpdateSchemas<T>();
            _applicationLifetime.StopApplication();

            return Task.CompletedTask;
        }
    }
}