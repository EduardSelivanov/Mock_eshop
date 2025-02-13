using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommonPractices.Carter
{
    public static class CarterConfiguration
    {
        public static IServiceCollection AddCarterConfig(this IServiceCollection services)
        {
            services.AddCarter();
            return services;
        }
        public static void UseCarterConfig(this WebApplication app)
        {
            app.MapCarter();
        }
    }
}
