using CommonPractices.RabbitMQContracts;
using MassTransit;


namespace Ordering.Application.Services
{
    public class RabbitService(IPublishEndpoint publishEndpoint,IRequestClient<CreateOrderAMQ> reqMQClient)
    {
        public async Task SendToRabbitMQ(CreateOrderAMQ crOrder)
        {
            var resp=await reqMQClient.GetResponse<SimpleMessage,OrderAcceptedMessage>(crOrder);

            if(resp.Is<SimpleMessage>(out var simpleMessage))
            {
                throw new Exception(simpleMessage.Message.SomeMessage);
            }

            if(resp.Is<OrderAcceptedMessage>(out var OrderAcceptedMessage))
            {
                throw new Exception($"Almost done, total price is {OrderAcceptedMessage.Message.TotalPrice}");
            }
        }
    }
}
