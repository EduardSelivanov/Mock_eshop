using HotChocolate;
using Ordering.Application.Services;
using Ordering.Domain.Models;

namespace Ordering.Presentation.GraphQL
{
    public class GraphQLEndpoints
    {
        
        public async Task<OrderModel> GetOrder([Service]  MartenService marSer,Guid id)
        {
            return await marSer.GetOrderById(id);
        }
    }
}
