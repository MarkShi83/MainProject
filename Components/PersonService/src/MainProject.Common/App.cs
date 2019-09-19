using System;
using System.IO;
using System.Threading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExtractService.Common {
    public class App {
        //private const string EnvironmentName = "NETCORE_ENVIRONMENT";

        //private const string AspEnvironmentName = "ASPNETCORE_ENVIRONMENT";

        //private IServiceCollection _serviceCollection;

        //private string _role;

        //public IServiceProvider ServiceProvider {
        //    get; set;
        //}

        //public IConfiguration Configuration {
        //    get; set;
        //}

        //public static App CreateWithDefaults(Action<IConfigurationBuilder> configure = null) {
        //    var app = new App();
        //    var cancellationTokenSource = new CancellationTokenSource();

        //    app.Configuration = Build(configure);
        //    app._serviceCollection = new ServiceCollection();

        //    app._serviceCollection
        //        .AddSingleton<Func<DateTime>>(() => DateTime.Now)
        //        .AddLogging(
        //            loggingBuilder =>
        //            {
        //                loggingBuilder
        //                    .AddConfiguration(app.Configuration.GetSection("Logging"))
        //                    .AddConsole(x => x.DisableColors = true);
        //            })
        //        .AddSingleton(app.Configuration)
        //        .AddSingleton(cancellationTokenSource);

        //    app._role = app._role ?? app.Configuration["entry:role"];

        //    return app;
        //}

        //public App SetRole(string role) {
        //    _role = role;
        //    return this;
        //}

        //public App Configure(Action<IServiceCollection, IConfiguration, string> configure) {
        //    if (!string.IsNullOrEmpty(_role)) {
        //        _role = _role.ToLowerInvariant();
        //    }

        //    configure?.Invoke(_serviceCollection, Configuration, _role);

        //    ServiceProvider = _serviceCollection.BuildServiceProvider();

        //    return this;
        //}

        //public void Start(Action<IServiceProvider, string> handler) {
        //    if (handler == null) {
        //        throw new ArgumentNullException(nameof(handler));
        //    }

        //    if (string.IsNullOrEmpty(_role)) {
        //        throw new InvalidOperationException("Unable to detect the entry point");
        //    }

        //    handler(ServiceProvider, _role);
        //}

        //private static IConfiguration Build(Action<IConfigurationBuilder> configure) {
        //    var environmentName = Environment.GetEnvironmentVariable(EnvironmentName)
        //        ?? Environment.GetEnvironmentVariable(AspEnvironmentName);

        //    var configurationBuilder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .AddJsonFile($"appsettings.{environmentName}.json", true);

        //    configure?.Invoke(configurationBuilder);

        //    configurationBuilder
        //        .AddEnvironmentVariables();

        //    return configurationBuilder.Build();
        //}
    }
}