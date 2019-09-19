using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MainProject.Common.Hosting
{
    public static class Extensions
    {
        public static IServiceCollection AddEssentials(this IServiceCollection services)
        {
            return services
                .AddSingleton<Func<DateTime>>(() => DateTime.Now);
        }
    }
}
