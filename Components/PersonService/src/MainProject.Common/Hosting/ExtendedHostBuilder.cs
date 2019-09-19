using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MainProject.Common.Hosting
{
    public class ExtendedHostBuilder
    {
        private readonly Dictionary<string, IStartupBase> _entryPoints;

        public ExtendedHostBuilder()
        {
            _entryPoints = new Dictionary<string, IStartupBase>();
        }

        public ExtendedHostBuilder Use<T>()
            where T : IStartupBase, new()
        {
            var startup = new T();
            var applicationName = startup.ApplicationName;

            if (_entryPoints.ContainsKey(applicationName))
            {
                _entryPoints[applicationName] = startup;
            }
            else
            {
                _entryPoints.Add(applicationName, startup);
            }

            return this;
        }

        public IHost Build()
        {
            var hostBuilder = new HostBuilder();

            hostBuilder
                .ConfigureHostConfiguration(
                    builder =>
                        {
                            builder.AddEnvironmentVariables(string.Empty);
                        })
                .ConfigureAppConfiguration(
                    (hostContext, configuration) =>
                        {
                            var environmentName = hostContext.HostingEnvironment.EnvironmentName;
                            var applicationName = hostContext.HostingEnvironment.ApplicationName;

                            configuration
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile($"appsettings.{environmentName}.json", true);

                            if (_entryPoints.First(e => e.Key == applicationName).Value is IStartup startup)
                            {
                                startup
                                    .ConfigureAppConfigurationDelegate?.Invoke(hostContext, configuration);
                            }
                        })
                .ConfigureLogging(
                    (context, loggingBuilder) =>
                        {
                            loggingBuilder
                                .AddConfiguration(context.Configuration.GetSection("Logging"))
                                .AddConsole(x => x.DisableColors = true);
                        })
                .ConfigureServices(
                    (context, services) =>
                        {
                            services.AddEssentials();

                            var applicationName = context.HostingEnvironment.ApplicationName;
                            if (!_entryPoints.ContainsKey(applicationName))
                            {
                                throw new InvalidOperationException($"Entry point for application name [{applicationName}] not found!");
                            }

                            var action = _entryPoints[applicationName];
                            action.Configure(services, context.Configuration);
                        });

            return hostBuilder.Build();
        }
    }
}
