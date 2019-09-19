using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MainProject.Common.Hosting
{
    public interface IStartupBase
    {
        string ApplicationName { get; }

        void Configure(IServiceCollection services, IConfiguration configuration);
    }
}
