using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MainProject.Common.Data
{
    public class TransactionManager : ITransactionManager
    {
        private readonly string _connectionString;

        private readonly DbProviderFactory _providerFactory;

        private IDbConnection _currentConnection;

        private IDbTransaction _currentTransaction;

        public TransactionManager(DbProviderFactory providerFactory, string connectionString)
        {
            _providerFactory = providerFactory;
            _connectionString = connectionString;
        }

        public void CommitTransaction()
        {
            if (_currentConnection == null)
            {
                throw new InvalidOperationException("Cannot commit a transaction that is not started");
            }

            try
            {
                _currentTransaction.Commit();
            }
            catch (Exception)
            {
                 _currentTransaction.Rollback();
                
                throw;
            }
            finally
            {
                _currentConnection.Close();
                _currentConnection.Dispose();
                
                _currentConnection = null;
                _currentTransaction = null;
            }
        }

        public void DoInTransaction(Action action)
        {
            DoInTransaction(
                connection => { action(); });
        }

        public void DoInTransaction(Action<IDbConnection> action)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                if (connection == null)
                {
                    throw new InvalidOperationException("Current connection is null.");
                }

                DbTransaction transaction = null;

                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    _currentConnection = connection;
                    _currentTransaction = transaction;

                    action(connection);

                    transaction.Commit();
                }
                catch
                {
                    transaction?.Rollback();
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    _currentConnection = null;
                    _currentTransaction = null;
                }
            }
        }

        public async Task DoInTransactionAsync(Func<Task> action)
        {
            using (var connection = _providerFactory.CreateConnection())
            {
                if (connection == null)
                {
                    throw new InvalidOperationException("Current connection is null.");
                }

                DbTransaction transaction = null;

                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    _currentConnection = connection;
                    _currentTransaction = transaction;

                    await action();

                    transaction.Commit();
                }
                catch
                {
                    transaction?.Rollback();
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    _currentConnection = null;
                    _currentTransaction = null;
                }
            }
        }

        public IDbConnection GetCurrentConnection()
        {
            if (_currentConnection == null)
            {
                throw new InvalidOperationException("Cannot get a connection that has not been started");
            }

            return _currentConnection;
        }

        public void RollbackTransaction()
        {
            if (_currentConnection == null)
            {
                throw new InvalidOperationException("Cannot rollback a transaction that is not started");
            }

            try
            {
                _currentTransaction.Rollback();
            }
            finally
            {
                _currentConnection.Close();
                _currentConnection.Dispose();
                
                _currentConnection = null;
                _currentTransaction = null;
            }
        }

        public void StartTransaction()
        {
            if (_currentConnection != null)
            {
                throw new InvalidOperationException("Nested Transactions are not allowed");
            }

            var connection = _providerFactory.CreateConnection();

            if (connection == null)
            {
                throw new InvalidOperationException("Current connection is null.");
            }

            connection.Open();
            var trans = connection.BeginTransaction();

            _currentConnection = connection;
            _currentTransaction = trans;
        }
    }
}
