using System;

using MainProject.Common.Data.Helpers;
using MainProject.Common.Hosting;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PersonService.Data;
using PersonService.WebApi.Configurations;

namespace PersonService.WebApi
{
    public class Startup
    {
        public string ApplicationName => "webapi";

        public Action<HostBuilderContext, IConfigurationBuilder> ConfigureAppConfigurationDelegate => null;

        public void Configure(IApplicationBuilder app)
        {
            app
                .UseMvc()
                .UseSwagger()
                .UseSwaggerUi();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<IConfiguration>();
            services
                .AddEssentials()
                .AddSwagger()
                .AddDataServices(config)
                .AddQueryHelpers()
                .AddMvc(options => options.RespectBrowserAcceptHeader = true)
                .AddJsonOptions(
                options =>
                    {
                        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    })
                .AddXmlSerializerFormatters();
        }
    }
}
