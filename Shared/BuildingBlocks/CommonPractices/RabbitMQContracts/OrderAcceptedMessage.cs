

namespace CommonPractices.RabbitMQContracts
{
    public record OrderAcceptedMessage(string orderNumber,decimal TotalPrice, Dictionary<string, int> basket, List<string> slots);
}
