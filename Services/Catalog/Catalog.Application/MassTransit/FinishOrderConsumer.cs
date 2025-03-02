

using CommonPractices.RabbitMQContracts;
using MassTransit;

namespace Catalog.Application.MassTransit
{
    class FinishOrderConsumer : IConsumer<FinishOrderMQ>
    {
        public async Task Consume(ConsumeContext<FinishOrderMQ> context)
        {
            var r = context.Message.slots;
            
        }
    }
}
