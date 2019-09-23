using System.Linq;
using System.Threading.Tasks;

using Dapper;

using MainProject.Common.Data.Helpers.Models;

namespace MainProject.Common.Data.Helpers
{
    public interface IQueryRunner
    {
        Task<PagedResult<T>> RunAsync<T, TQuery>(TQuery query) where TQuery : Query;
    }

    public class QueryRunner : IQueryRunner
    {
        private readonly ITransactionManager _transactionManager;

        private readonly IQueryBuilder _queryBuilder;

        public QueryRunner(ITransactionManager transactionManager, IQueryBuilder queryBuilder)
        {
            _transactionManager = transactionManager;
            _queryBuilder = queryBuilder;
        }

        public async Task<PagedResult<T>> RunAsync<T, TQuery>(TQuery query) where TQuery : Query
        {
            var result = new PagedResult<T>();

            await _transactionManager
                .DoInTransactionAsync(
                    async () =>
                    {
                        var connection = _transactionManager.GetCurrentConnection();
                        var compiledQuery = _queryBuilder.Build(query);
                        object parameters = compiledQuery.Parameters;

                        if (query.Size.Value > 0)
                        {
                            result.Data = (await connection.QueryAsync<T>(compiledQuery.Sql, parameters)).ToList();
                        }

                        result.Total = await connection.QueryFirstOrDefaultAsync<long>(compiledQuery.CountSql, parameters);

                        result.PageSize = query.Size.Value;
                        result.CurrentPage = query.Page.Value;
                    });

            return result;
        }
    }
}
