using CommonPractices.Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WareHouse.Application.CQRS.RackCQRS.Commands;

namespace WareHouse.Presentation.Endpoints.Rack
{
    public class CreateRackEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/racks", async (CreateRackComm rack,ISender sender) =>
            {
                string res = await sender.Send(rack);
                return Results.Ok(res);

            });
        }
    }
}
