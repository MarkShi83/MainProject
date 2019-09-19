using System;

using MainProject.Common.Hosting;
using MainProject.Common.Rest;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PersonService.ConsoleApp
{
    public class Startup : IStartup
    {
        public string ApplicationName => "consoleapp";

        public Action<HostBuilderContext, IConfigurationBuilder> ConfigureAppConfigurationDelegate => null;

        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddRestServices(configuration)
                .AddHostedService<ConsoleAppService>();
        }
    }
}