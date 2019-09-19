using MainProject.Common.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PersonService.Data.Impl;

namespace PersonService.Data
{
    public static class Configuration
    {
        public static IServiceCollection AddDataServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddCoreDataServices(configuration)
                .AddTransient<IPersonDataService, PersonDataService>();

            return services;
        }
    }
}
