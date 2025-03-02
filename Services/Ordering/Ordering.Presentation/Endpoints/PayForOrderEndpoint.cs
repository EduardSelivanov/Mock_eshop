using CommonPractices.Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Ordering.Application.Services;


namespace Ordering.Presentation.Endpoints
{
    public record OrderPayment(string orderId,decimal summTopay);
    class PayForOrderEndpoint : IEndpoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/ordering/pay", async (RabbitService service) =>
            {
                OrderPayment payment = new OrderPayment("1",1);
                var f = await service.SendPayment(payment.orderId, payment.summTopay);
            });
        }
    }
}
