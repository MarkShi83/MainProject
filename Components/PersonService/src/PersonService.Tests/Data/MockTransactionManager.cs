using System;
using System.Data;
using System.Threading.Tasks;

using MainProject.Common.Data;

namespace PersonService.Tests.Data
{
    public class MockTransactionManager : ITransactionManager
    {
        public bool RaiseError { get; set; }

        public IDbConnection GetCurrentConnection()
        {
            return null;
        }

        public void StartTransaction()
        {
        }

        public void CommitTransaction()
        {
        }

        public void RollbackTransaction()
        {
        }

        public void DoInTransaction(Action action)
        {
            action();
        }

        public void DoInTransaction(Action<IDbConnection> action)
        {
            action(null);
        }

        public async Task DoInTransactionAsync(Func<Task> action)
        {
            if (RaiseError)
            {
                throw new InvalidOperationException();
            }

            await action();
        }
    }
}
