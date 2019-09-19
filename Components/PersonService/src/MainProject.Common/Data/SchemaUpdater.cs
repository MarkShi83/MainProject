using DbUp.Postgresql;

using MainProject.Common.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MainProject.Common.Data
{
    public class SchemaUpdater : ISchemaUpdater
    {
        private readonly DataOptions _dataOptions;

        private readonly ILogger<SchemaUpdater> _logger;

        public SchemaUpdater(ILogger<SchemaUpdater> logger, IOptions<DataOptions> dataOptions)
        {
            _logger = logger;
            _dataOptions = dataOptions.Value;
        }

        public void UpdateSchemas<T>()
        {
            DbUp.EnsureDatabase.For.PostgresqlDatabase(_dataOptions.ConnectionString);

            var config = DbUp
                .DeployChanges
                .To
                .PostgresqlDatabase(_dataOptions.ConnectionString)
                .LogScriptOutput()
                .LogToConsole()
                .WithScriptsEmbeddedInAssembly(typeof(T).Assembly, x => x.EndsWith(".sql"))
                .WithTransactionPerScript();

            config.Configure(
                c => c.Journal = new PostgresqlTableJournal(
                         () => c.ConnectionManager,
                         () => c.Log,
                         null,
                         _dataOptions.JournalTable));

            var upgrader = config.Build();
            var results = upgrader.PerformUpgrade();

            if (!results.Successful)
            {
                _logger.LogError(results.Error.ToString());
                return;
            }

            foreach (var script in results.Scripts)
            {
                _logger.LogInformation($"{script.Name}: Done");
            }
        }
    }
}
