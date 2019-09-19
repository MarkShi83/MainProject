using System;
using System.Threading.Tasks;

using MainProject.Common.Hosting;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PersonService
{
    public static class Program
    {
        public static IHost Host { get; private set; }

        public static IWebHost LocalWebHost { get; private set; }

        public static async Task Main()
        {
            if (Environment.GetEnvironmentVariable("APPLICATIONNAME")?.ToLower() == "webapi")
            {
                LocalWebHost = WebHost.CreateDefaultBuilder()
                    .UseStartup<WebApi.Startup>()
                    .Build();

                await LocalWebHost.RunAsync();
                return;
            }

            Host = new ExtendedHostBuilder()
                .Use<ConsoleApp.Startup>()
                .Use<SchemaUpdater.Startup>()
                .Build();

            await Host.RunAsync();
        }
    }
}
