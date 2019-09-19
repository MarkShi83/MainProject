using System;

using MainProject.Common.Data;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace PersonService.Tests.Data
{
    public class BaseDatabaseTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly DatabaseFixture _databaseFixture;

        public BaseDatabaseTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            CallScope = databaseFixture.ServiceProvider.CreateScope();
            TransactionManager = CallScope.ServiceProvider.GetService<ITransactionManager>();
        }

        ~BaseDatabaseTests()
        {
            Dispose(false);
        }

        protected IServiceScope CallScope { get; }

        protected ITransactionManager TransactionManager { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            var tables = new[]
            {
                "person"
            };

            _databaseFixture.ResetDataInTestDatabase(tables);
            CallScope.Dispose();
        }
    }
}