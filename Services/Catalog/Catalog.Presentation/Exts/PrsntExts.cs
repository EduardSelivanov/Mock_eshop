using Carter;
using Catalog.Presentation.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Presentation.Exts
{
    public static class PrsntExts
    {
        public static void AddPrsnt(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddSingleton<ICarterModule, CreateProductEndpoint>();
        }
    }
}
