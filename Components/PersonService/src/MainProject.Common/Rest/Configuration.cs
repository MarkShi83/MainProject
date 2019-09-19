using System;
using System.Net.Http;
using MainProject.Common.Models.Rest;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MainProject.Common.Rest
{
    public static class Configuration
    {
        public static IServiceCollection AddRestServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<RestOptions>(configuration.GetSection("rest"))
                .AddTransient<IRestClient, RestClient>()
                .AddTransient<IPersonServiceApi, PersonServiceApi>()
                .AddSingleton(serviceProvider =>
                {
                    var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(1600) };
                    return httpClient;
                });
        }
    }
}
