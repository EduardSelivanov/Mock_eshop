using Carter;
using Microsoft.Extensions.DependencyInjection;
using WareHouse.Presentation.Endpoints.Rack;

namespace WareHouse.Presentation.Exts
{
    public static class PresentationExts
    {
      public static void AddPresentation(this IServiceCollection services)
        {
            //services.AddCarterConfig();
            services.AddSingleton<ICarterModule, CreateRackEndpoint>();
            services.AddSingleton<ICarterModule, GetRackEndpoint>();
            services.AddSingleton<ICarterModule, EditRackEndpoint>();
            services.AddSingleton<ICarterModule, GetRacksEndpoint>();
            services.AddSingleton<ICarterModule, DeleteRackEndpoint>();


        }

    }
}
