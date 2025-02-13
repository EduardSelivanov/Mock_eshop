using CommonPractices.Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using WareHouse.Application.CQRS.RackCQRS.Queries;

namespace WareHouse.Presentation.Endpoints.Rack
{
    internal class GetRacksEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/racks", async ([AsParameters] GetRacksQuery req, ISender sender) =>
            {
                var racks = await sender.Send(req);

                var racksDTO = racks.Select(x => new
                {
                    RackName=x.RackName,
                    RacDesc=x.RackDescription,
                    RackSpaces=x.TotalPlaces
                }).ToList();
                return Results.Ok(racksDTO);                
            });
        }
    }
}
