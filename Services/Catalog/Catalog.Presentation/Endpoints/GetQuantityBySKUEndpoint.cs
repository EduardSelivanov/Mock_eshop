using Catalog.Application.CQRS.ProductCQRS.Queries;
using CommonPractices.Carter;
using CommonPractices.ResultHandler;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Presentation.Endpoints
{
    class GetQuantityBySKUEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/products/{sku}", async (string sku,ISender sender) =>
            {
                CustomResult<int> res = await sender.Send(new GetQuantityBySKU(sku));
                return Results.Ok(res.SuccessResponse);
            });
        }
    }
}
