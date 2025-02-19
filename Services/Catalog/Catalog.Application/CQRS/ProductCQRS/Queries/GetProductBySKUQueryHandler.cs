
using Catalog.Domain.Models;
using CommonPractices.CQRS;
using Marten;

namespace Catalog.Application.CQRS.ProductCQRS.Queries
{
    public record GetProdBySKUQuery(string sku):ICommandCP<Product>;
    class GetProductBySKUQueryHandler(IDocumentSession session) : ICommHandBase<GetProdBySKUQuery, Product>
    {
        public async Task<Product> Handle(GetProdBySKUQuery request, CancellationToken cancellationToken)
        {
            Product prod = session.Query<Product>().FirstOrDefault(prod=>prod.SKU.Equals(request.sku));
            if (prod == null)
            {
                return null;
            }
            return prod;
        }
    }
}
