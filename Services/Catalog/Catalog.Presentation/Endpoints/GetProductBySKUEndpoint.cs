
using Catalog.Application.CQRS.ProductCQRS.Queries;
using CommonPractices.Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Presentation.Endpoints
{
    class GetProductBySKUEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/catalog",async (GetProdBySKUQuery req,ISender sender) =>
            {
                var tos = await sender.Send(req);
                return Results.Ok();
            });
        }
    }
}
