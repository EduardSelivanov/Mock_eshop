using CommonPractices.RabbitMQContracts;
using CommonPractices.ResultHandler;
using Marten;
using MassTransit;
using Ordering.Domain.Models;
using System.Security.Cryptography.Xml;


namespace Ordering.Application.Services
{
    public class RabbitService(IPublishEndpoint publishEndpoint,IDocumentSession session,IRequestClient<CreateOrderAMQ> reqMQClient,PaymentService paymentSer)
    {
        public async Task SendRequestForOrderMQ(CreateOrderAMQ crOrder)
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
                    Basket=OrderAcceptedMessage.Message.basket,
                    TotalPrice=OrderAcceptedMessage.Message.TotalPrice
                };
                session.Store(newOrder);
                await session.SaveChangesAsync();

                throw new Exception($"Almost done, total price is {OrderAcceptedMessage.Message.TotalPrice}, please pay for your order {OrderAcceptedMessage.Message.orderNumber}");
            }
        }

        public async Task<CustomResult<string>> SendPayment(string orderId,decimal summ)
        {
            var d=await paymentSer.ProceedPayment(summ,orderId);
            if (d.Item1 == false)
            {
                return CustomResult<string>.Failure("Error during payment");
            }
            FinishOrderMQ finalStep = new FinishOrderMQ(d.Item2);
            var resp = await reqMQClient.GetResponse<SimpleMessage, PaymentConfirmedMQ>(finalStep);

            return null;

        }
    }
}
