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
            decimal totalSum = 0;


            foreach (OrderItemAMQ ord in res)
            {
                var p = session.Query<Product>().FirstOrDefault(prod => prod.SKU.Equals(ord.SKU));

                GetSlotBySKUReq req = new GetSlotBySKUReq(){SKU=ord.SKU,Count=ord.Quantity};
                try
                {
                    GetSlotBySKUResp response = await _grpc.GetSlotBySKUAsync(req);
                    if (response.Total < ord.Quantity)
                    {
                        await context.RespondAsync(new SimpleMessage("Some goods arnt available"));
                    }
                    totalSum = totalSum + (response.SlotId.Count * p.Price);
                    goodsForOrder.AddRange(response.SlotId);
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
                {
                    await context.RespondAsync(new SimpleMessage("No prods in warehouse"));
                }
            }

            //create Order
            foreach(var id in goodsForOrder)
            {
                EditSlotByIDReq req = new EditSlotByIDReq() {SlotId=id};
                EditSlotByIdResp resp = await _grpc.EditSlotByIDAsync(req);
                if (!resp.Success)
                {
                    await context.RespondAsync(new SimpleMessage("Some ErrorWith GRPC"));
                }
                await context.RespondAsync(new OrderAcceptedMessage(totalSum,Guid.NewGuid().ToString()));
            }
            //


        }
    }
}
