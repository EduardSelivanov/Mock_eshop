
using Catalog.Application.CQRS.ProductCQRS.Commands;
using CommonPractices.Carter;
using CommonPractices.ResultHandler;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Presentation.Endpoints
{
    public class CreateProductEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/products", async ([FromBody] CreateProdComm prod, ISender sender) =>
            {
                CustomResult<string> result = await sender.Send(prod);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result.ErrorMessage);
                }
                return Results.Ok(result.SuccessResponse);
            });
        }
    }
}
