using CommonPractices.RabbitMQContracts;
using CommonPractices.ResultHandler;
using Marten;
using MassTransit;
using Ordering.Domain.Models;


namespace Ordering.Application.Services
{
    public class RabbitService(IPublishEndpoint publishEndpoint,IDocumentSession session,IRequestClient<CreateOrderAMQ> reqMQClient,PaymentService paymentSer)
    {
        public async Task<string> SendRequestForOrderMQ(CreateOrderAMQ crOrder)
        {
            //CreateOrderAMQ
            var resp =await reqMQClient.GetResponse<SimpleMessage,OrderAcceptedMessage>(crOrder);

            if(resp.Is<SimpleMessage>(out var simpleMessage))
            {
                throw new Exception(simpleMessage.Message.SomeMessage);
            }

            if(resp.Is<OrderAcceptedMessage>(out var OrderAcceptedMessage))
            {

                OrderModel newOrder = new OrderModel()
                {
                    Id=Guid.Parse(OrderAcceptedMessage.Message.orderNumber),
                    Slots=OrderAcceptedMessage.Message.slots,
                    Basket=OrderAcceptedMessage.Message.basket
                    .Select(kv=>new BasketItem { Sku=kv.Key,Quantity=kv.Value}).ToList(),
                    TotalPrice=OrderAcceptedMessage.Message.TotalPrice
                };
                session.Store(newOrder);
                await session.SaveChangesAsync();

                return $"Almost done, total price is {OrderAcceptedMessage.Message.TotalPrice}, please pay for your order {OrderAcceptedMessage.Message.orderNumber}";
            }
            return null;
        }

        public async Task<CustomResult<string>> SendPayment(string orderId, decimal summ)
        {
            var d = await paymentSer.ProceedPayment(summ, orderId);
            if (d.Item1 == false)
            {
                return CustomResult<string>.Failure("Error during payment");
            }
            FinishOrderMQ finalStep = new FinishOrderMQ(d.Item2);
            var resp = await reqMQClient.GetResponse<SimpleMessage, PaymentConfirmedMQ>(finalStep);

            if (resp.Is<SimpleMessage>(out var simpleMessage))
            {
                return CustomResult<string>.Failure("Some error, try later ");
            }
            if (resp.Is<PaymentConfirmedMQ>(out var paymentConfirmed))
            {
                var f = paymentConfirmed.Message.orderId;
                return CustomResult<string>.Success($"Your order {f} was payed");
            }
            return CustomResult<string>.Failure("Some error... Try later");
        }
    }
}
