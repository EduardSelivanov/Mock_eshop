

using HotChocolate.Types;
using Ordering.Domain.Models;

namespace Ordering.Presentation.GraphQL
{
    public class BasketType : ObjectType<BasketItem>
    {
        protected override void Configure(IObjectTypeDescriptor<BasketItem> descriptor)
        {
            descriptor.Field(f => f.Sku).Type<StringType>();
            descriptor.Field(f => f.Quantity).Type<IntType>();
        }
    }
}
