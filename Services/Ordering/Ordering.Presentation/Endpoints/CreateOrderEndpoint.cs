using CommonPractices.Carter;
using CommonPractices.RabbitMQContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Ordering.Application.Services;

namespace Ordering.Presentation.Endpoints
{
    class CreateOrderEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/ordering", async (RabbitService service) =>
            {
                try
                {
                    CreateOrderAMQ orderspecs = 
                    new CreateOrderAMQ(new List<OrderItemAMQ>{new OrderItemAMQ("first", 2)});

                    await service.SendToRabbitMQ(orderspecs);

                    return Results.Ok("Order created successfully!");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = ex.Message }); 
                }
            });
        }
    }
}
