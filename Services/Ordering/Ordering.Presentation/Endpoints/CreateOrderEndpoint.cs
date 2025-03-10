﻿using CommonPractices.Carter;
using CommonPractices.RabbitMQContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Ordering.Application.Services;
using Ordering.Domain.Models;

namespace Ordering.Presentation.Endpoints
{
    class CreateOrderEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/ordering", async (RabbitService service) =>
            {
                
                    CreateOrderAMQ orderspecs = 
                    new CreateOrderAMQ(new List<OrderItemAMQ>{new OrderItemAMQ("first", 1)});

                    await service.SendRequestForOrderMQ(orderspecs);

                    return Results.Ok("Order created successfully!");
               
            });
        }
    }
}
