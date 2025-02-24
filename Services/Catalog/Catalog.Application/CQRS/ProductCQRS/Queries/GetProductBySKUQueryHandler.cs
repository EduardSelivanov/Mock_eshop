using Catalog.Domain.Models;
using CommonPractices.CQRS;
using CommonPractices.ResultHandler;
using Marten;
using SlotsService;

namespace Catalog.Application.CQRS.ProductCQRS.Queries
{
    public record GetProdBySKUQuery(string sku):ICommandCP<CustomResult<string>>;
    class GetProductBySKUQueryHandler(IDocumentSession session,SlotsProtoService.SlotsProtoServiceClient _grpcClient) : ICommHandBase<GetProdBySKUQuery, CustomResult<string>>
    {
        public async Task<CustomResult<string>> Handle(GetProdBySKUQuery request, CancellationToken cancellationToken)
        {
            Product prod = session.Query<Product>().FirstOrDefault(prod=>prod.SKU.Equals(request.sku));
            if (prod == null)
            {
                return CustomResult<string>.Failure("no product found");
            }
            GetSlotBySKUResp resonse = await _grpcClient.GetSlotBySKUAsync(new GetSlotBySKUReq() {SKU=prod.SKU});
            if (resonse == null)
            {
                return CustomResult<string>.Failure("No products on warehouse");
            }

            string responseToClient = $"Your produsct {request.sku} is on Rack {resonse.RackId} on place {resonse.Place}";


            return CustomResult<string>.Success(responseToClient);
        }
    }
}
