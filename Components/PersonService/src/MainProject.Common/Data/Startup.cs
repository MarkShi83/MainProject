using System;

using MainProject.Common.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MainProject.Common.Data
{
    public class Startup<T> : IStartup
    {
        public string ApplicationName => "schemaupdater";

        public Action<HostBuilderContext, IConfigurationBuilder> ConfigureAppConfigurationDelegate => null;

        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCoreDataServices(configuration)
                .AddHostedService<SchemaUpdateService<T>>();
        }
    }
}