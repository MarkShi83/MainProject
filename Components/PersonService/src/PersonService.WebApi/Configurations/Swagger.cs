using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;

namespace PersonService.WebApi.Configurations
{
    public static class Swagger
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(
                c =>
                    {
                        c.SwaggerDoc("v1", new Info { Title = "Person Service Api", Version = "v1" });
                    });
        }

        public static IApplicationBuilder UseSwaggerUi(this IApplicationBuilder app)
        {
            return app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(string.Format("/{0}/v1/swagger.json", "swagger"), "Person Service Api V1");
            });
        }
    }
}
