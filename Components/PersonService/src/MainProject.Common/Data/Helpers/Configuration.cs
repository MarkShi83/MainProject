using Microsoft.Extensions.DependencyInjection;

namespace MainProject.Common.Data.Helpers
{
    public static class Configuration
    {
        public static IServiceCollection AddQueryHelpers(this IServiceCollection services)
        {
            return services
                .AddTransient<IQueryParameterParser, QueryParameterParser>()
                .AddTransient<IQueryBuilder, QueryBuilder>()
                .AddTransient<IQueryRunner, QueryRunner>();
        }
    }
}
