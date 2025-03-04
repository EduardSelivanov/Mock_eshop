using Carter;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Presentation.Endpoints;
using Ordering.Presentation.GraphQL;

namespace Ordering.Presentation.Extensions
{
    public static class PresentationExt
    {
        public static void AddPresentation(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddSingleton<ICarterModule, CreateOrderEndpoint>();
            services.AddSingleton<ICarterModule, PayForOrderEndpoint>();

            services.AddGraphQLServer()
                .AddQueryType<GraphQLEndpoints>()
                .AddMutationType<Mutations>()
                ;// fix
        }
    }
}
