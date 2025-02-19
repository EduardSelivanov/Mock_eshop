
using Catalog.Application.CQRS.ProductCQRS.Commands;
using CommonPractices.Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Presentation.Endpoints
{
    public class CreateProductEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/products", async (CreateProdComm prod, ISender sender) =>
            {
                string res = await sender.Send(prod);
                return Results.Ok();
            });
        }
    }
}
