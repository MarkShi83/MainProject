using System;
using System.Data;
using System.Threading.Tasks;

namespace MainProject.Common.Data
{
    public interface ITransactionManager
    {
        IDbConnection GetCurrentConnection();

        void StartTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        void DoInTransaction(Action action);

        void DoInTransaction(Action<IDbConnection> action);

        Task DoInTransactionAsync(Func<Task> action);
    }
}
