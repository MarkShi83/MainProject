using System;
using System.Linq;

using FluentAssertions;

using MainProject.Common.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PersonService.SchemaUpdater;

using Xunit;

namespace PersonService.Tests
{
    public class ConfigurationTests
    {
        [Theory]
        [InlineData("schemaupdater", typeof(Startup))]
        [InlineData("wepapi", typeof(PersonService.WebApi.Startup))]
        public void WhenServicesAreAddedThenAllTheInterfacesCanBeResolved(
            string role,
            Type type,
            Type additionalType = null)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var serviceCollection = new ServiceCollection()
                .AddLogging()
                .AddEssentials();

            var startup = Activator.CreateInstance(type) as IStartup;
            startup?.ApplicationName.Should().Be(role);
            startup?.Configure(serviceCollection, configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var avoidInterfaces = new Type[0];

            var interfaces = type.Assembly.GetTypes()
                                            .Where(c => c.IsInterface && c.IsPublic)
                                            .Where(c => avoidInterfaces.All(i => c != i))
                                            .ToArray();

            foreach (var assembly in interfaces)
            {
                serviceProvider.GetServices(assembly).Should().NotBeEmpty($"{assembly.Name} can't be resolved");
            }

            if (additionalType == null)
            {
                return;
            }

            serviceProvider.GetServices(additionalType).Should().NotBeEmpty();
        }
    }
}