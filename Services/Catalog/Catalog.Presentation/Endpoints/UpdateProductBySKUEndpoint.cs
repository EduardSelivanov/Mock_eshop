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
    class UpdateProductBySKUEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("api/products",async([FromBody] UpdateProductBySKU upds,ISender sender) =>
            {
                CustomResult<string> res = await sender.Send(upds);
                

                return res.IsSuccess?Results.Ok(res.SuccessResponse):Results.BadRequest(res.ErrorMessage);
            });
        }
    }
}
