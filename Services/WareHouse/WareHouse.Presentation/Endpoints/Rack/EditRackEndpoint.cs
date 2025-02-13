using CommonPractices.Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WareHouse.Application.CQRS.RackCQRS.Commands;

namespace WareHouse.Presentation.Endpoints.Rack
{
    internal class EditRackEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapPatch("/api/racks/{id}", async (Guid id,RackDto rack,ISender sender) =>
            {
                EditRackComm comm = 
                new EditRackComm(id,rack.RackName,rack.RackDescription,rack.RackX,rack.RackY);

                bool res=await sender.Send(comm);

                return Results.Ok(res);

            });
        }
    }
}
