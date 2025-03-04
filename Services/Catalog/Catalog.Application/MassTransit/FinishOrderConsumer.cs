using CommonPractices.RabbitMQContracts;
using MassTransit;
using SlotsService;

namespace Catalog.Application.MassTransit
{
    class FinishOrderConsumer(SlotsProtoService.SlotsProtoServiceClient _grpc,IPublishEndpoint _publishEndpoint) : IConsumer<FinishOrderMQ>
    {
        public async Task Consume(ConsumeContext<FinishOrderMQ> context)
        {
            List<string> slotsToEmpty = context.Message.slots;
            // need to change slots 
            // mb send email verification 
            // add PickedUp option Order
            // remove carter 
        }
    }
}
