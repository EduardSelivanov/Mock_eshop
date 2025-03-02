using Marten;
using Ordering.Domain.Models;

namespace Ordering.Application.Services
{
    public class PaymentService(IDocumentSession session)
    {
        public async Task<(bool,List<string>)> ProceedPayment(decimal sum, string orderId)
        {
            var order = await session.LoadAsync<OrderModel>(Guid.Parse(orderId));
            if (order.TotalPrice == sum)
            {
                return (true,order.Slots);
            }

            return (false,null);
        }
    }
}
