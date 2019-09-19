using System;

using MainProject.Common.Data;
using MainProject.Common.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PersonService.Data.Migration;

namespace PersonService.SchemaUpdater
{
    public class Startup : IStartup
    {
        public string ApplicationName => "schemaupdater";

        public Action<HostBuilderContext, IConfigurationBuilder> ConfigureAppConfigurationDelegate => null;

        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCoreDataServices(configuration)
                .AddHostedService<SchemaUpdateService<Anchor>>();
        }
    }
}