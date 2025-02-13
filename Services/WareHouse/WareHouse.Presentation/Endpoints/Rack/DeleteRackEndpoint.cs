
using CommonPractices.Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WareHouse.Application.CQRS.RackCQRS.Commands;

namespace WareHouse.Presentation.Endpoints.Rack
{
    internal class DeleteRackEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/racks{id}", async (Guid id, ISender sender) =>
            {
                DeleteRackComm deleteComm = new DeleteRackComm(id);

                string nameOfDeleted = await sender.Send(deleteComm);
                return Results.Ok(nameOfDeleted);
            });
        }
    }
}
