using Catalog.Domain.Models;
using CommonPractices.RabbitMQContracts;
using Grpc.Core;
using Marten;
using MassTransit;
using SlotsService;

namespace Catalog.Application.MassTransit
{
    class OrderCreatedConsumer(SlotsProtoService.SlotsProtoServiceClient _grpc, IDocumentSession session, IPublishEndpoint _publishEndpoint) : IConsumer<CreateOrderAMQ>
    {
        public async Task Consume(ConsumeContext<CreateOrderAMQ> context)
        {
            List<OrderItemAMQ> res = context.Message.orderReq; // sku quant
            List<string> goodsForOrder = new List<string>();
            Dictionary<string, int> basket = new Dictionary<string, int>();
            decimal totalSum = 0;


            foreach (OrderItemAMQ ord in res)
            {
                var p = session.Query<Product>().FirstOrDefault(prod => prod.SKU.Equals(ord.SKU));
                basket[ord.SKU] = ord.Quantity;

                GetSlotBySKUReq req = new GetSlotBySKUReq(){SKU=ord.SKU,Count=ord.Quantity};
                try
                {
                    GetSlotBySKUResp response = await _grpc.GetSlotBySKUAsync(req);
                    if (response.Total < ord.Quantity)
                    {
                        await context.RespondAsync(new SimpleMessage("Some goods arnt available"));
                        return;
                    }
                    totalSum = totalSum + (response.SlotId.Count * p.Price);
                    goodsForOrder.AddRange(response.SlotId);
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
                {
                    await context.RespondAsync(new SimpleMessage("No prods in warehouse"));
                    return;
                }
            }

            //create Order
            foreach(var id in goodsForOrder)
            {
                ReserveSlotIDReq req = new ReserveSlotIDReq() {SlotId=id};
                ReserveSlotIDResp resp = await _grpc.ReserveSlotIDAsync(req);
                if (!resp.Success)
                {
                    await context.RespondAsync(new SimpleMessage("Some ErrorWith GRPC"));
                    return;
                }
            }
            await context.RespondAsync(new OrderAcceptedMessage(Guid.NewGuid().ToString(), totalSum, basket, goodsForOrder));
            // todo
            //
            //fix Exceptions in Ordering
            // SOMEHOW SOLVE SLOW RESPONSES!!



        }
    }
}
