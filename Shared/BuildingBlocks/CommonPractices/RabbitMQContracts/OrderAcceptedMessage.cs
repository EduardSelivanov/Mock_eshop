

namespace CommonPractices.RabbitMQContracts
{
    public record OrderAcceptedMessage(decimal TotalPrice,string orderNumber);
}
