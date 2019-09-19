using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MainProject.Common.Hosting
{
    public interface IStartup : IStartupBase
    {
        Action<HostBuilderContext, IConfigurationBuilder> ConfigureAppConfigurationDelegate { get; }
    }
}