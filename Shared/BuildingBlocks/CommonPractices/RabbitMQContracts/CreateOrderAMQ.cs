

namespace CommonPractices.RabbitMQContracts
{
    public record CreateOrderAMQ(List<OrderItemAMQ> orderReq);
    
}
