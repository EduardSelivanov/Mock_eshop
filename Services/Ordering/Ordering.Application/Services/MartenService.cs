using Marten;
using Ordering.Domain.Models;

namespace Ordering.Application.Services
{
    public class MartenService(IDocumentSession session)
    {
        public async Task<OrderModel> GetOrderById(Guid id)
        {
            return await session.LoadAsync<OrderModel>(id);
        }

    }
}
