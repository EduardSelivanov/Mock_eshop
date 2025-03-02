using Carter;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Presentation.Endpoints;

namespace Ordering.Presentation.Extensions
{
    public static class PresentationExt
    {
        public static void AddPresentation(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddSingleton<ICarterModule, CreateOrderEndpoint>();
            services.AddSingleton<ICarterModule, PayForOrderEndpoint>();
        }
    }
}
