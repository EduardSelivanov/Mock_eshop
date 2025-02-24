using Catalog.Application.CQRS.ProductCQRS.Queries;
using CommonPractices.Carter;
using CommonPractices.ResultHandler;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Presentation.Endpoints
{
    class GetProductBySKUEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/products", async ([FromBody] GetProdBySKUQuery req,ISender sender) =>
            {
                CustomResult<string> result = await sender.Send(req);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.ErrorMessage);
                }
                return Results.Ok(result.SuccessResponse);
            });
        }
    }
}
