using System;

using DbUp.Postgresql;

using MainProject.Common.Data.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Npgsql;

namespace MainProject.Common.Data.Updater
{
    public class DataSchemaUpdater
    {
        private readonly DataOptions _dataOptions;

        private readonly ILogger<DataSchemaUpdater> _logger;

        public DataSchemaUpdater(ILogger<DataSchemaUpdater> logger, IOptions<DataOptions> dataOptions)
        {
            _logger = logger;
            _dataOptions = dataOptions.Value;
        }

        public void DropDatabase()
        {
            var masterConnectionStringBuilder = new NpgsqlConnectionStringBuilder(_dataOptions.ConnectionString);
            var databaseName = masterConnectionStringBuilder.Database;

            masterConnectionStringBuilder.Database = "postgres";

            NpgsqlConnection.ClearPool(new NpgsqlConnection(_dataOptions.ConnectionString));

            using (var connection = new NpgsqlConnection(masterConnectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var sqlCommandText = $"drop database if exists \"{databaseName}\";";

                // Drop the database...
                using (var command = new NpgsqlCommand(sqlCommandText, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ResetDataInTestDatabase(params string[] tableNames)
        {
            if (tableNames == null || tableNames.Length == 0)
            {
                return;
            }

            using (var connection = new NpgsqlConnection(_dataOptions.ConnectionString))
            {
                connection.Open();

                foreach (var tableName in tableNames)
                {
                    var commandText = $"delete from {tableName};";

                    using (var command = new NpgsqlCommand(commandText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
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