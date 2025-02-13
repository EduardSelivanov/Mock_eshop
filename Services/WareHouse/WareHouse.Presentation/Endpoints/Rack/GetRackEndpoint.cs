using CommonPractices.Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WareHouse.Application.CQRS.RackCQRS.Queries;
using WareHouse.Domain.Models;

namespace WareHouse.Presentation.Endpoints.Rack
{
    internal class GetRackEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/racks/{id}", async (Guid id,ISender sender)=>
            {
                GetRackByIdQuery query = new GetRackByIdQuery(id);

                RackModel rack = await sender.Send(query);
                var rackDTO = new
                {
                    rackNameDTO=rack.RackName,
                    rackDescDTO = rack.RackDescription,
                    rackTotakDTO= rack.TotalPlaces
                };
                return Results.Ok(rackDTO);
            });
        }
    }
}
