using CommonPractices.RabbitMQContracts;
using HotChocolate;
using Ordering.Application.Services;
using Ordering.Domain.Models;

namespace Ordering.Presentation.GraphQL
{
    public class Mutations
    {
        public async Task<string> CreateOrder([Service] RabbitService rabSer,List<BasketItem> bask)
        {
            var createOrderReq =
                new CreateOrderAMQ(bask.Select(bas=>new OrderItemAMQ(bas.Sku,bas.Quantity)).ToList());

            var resp = await rabSer.SendRequestForOrderMQ(createOrderReq);
            return resp;
        }
        public async Task<string> PayOrder([Service] RabbitService rabser,string orderId,decimal sum)
        {
            var s = await rabser.SendPayment(orderId,sum);
            return null;
        }
    }
}
