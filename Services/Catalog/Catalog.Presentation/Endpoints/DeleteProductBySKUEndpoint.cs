using Catalog.Application.CQRS.ProductCQRS.Commands;
using CommonPractices.Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Presentation.Endpoints
{
    class DeleteProductBySKUEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/products", async ([FromBody] DeleteProdBySKU req,ISender sender) =>
            {
                var r = await sender.Send(req);

                return r.IsSuccess ?Results.Ok(r.SuccessResponse):Results.BadRequest("Something wetn wrong");
            });
        }
    }
}
