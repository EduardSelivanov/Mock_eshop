using Catalog.Domain.Models;
using CommonPractices.CQRS;
using CommonPractices.ResultHandler;
using Marten;
using SlotsService;

namespace Catalog.Application.CQRS.ProductCQRS.Commands
{
    public record CreateProdComm(string sku,List<string> category,string description,decimal price):ICommandCP<CustomResult<string>>;

    public class CreateProdCommHandler(IDocumentSession session,SlotsProtoService.SlotsProtoServiceClient _grpcClient) : ICommHandBase<CreateProdComm, CustomResult<string>>
    {

        public async Task<CustomResult<string>> Handle(CreateProdComm request, CancellationToken cancellationToken)
        {

            Product newProd = new Product()
            {
                SKU=request.sku,
                Category=request.category,
                Description=request.description,
                Price=request.price
            };

      

            AssignProdReq toWarehouse = new AssignProdReq()
            {
                SKU=request.sku,
                Date= Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
            };

            AssignProdResp resp = await _grpcClient.AssignProductToSlotAsync(toWarehouse);
            if (!resp.Succes)
            {
                return CustomResult<string>.Failure("No free space in warehouse.");
            }

            session.Store(newProd);
            return CustomResult<string>.Success("Added to warehouse");
        }
    }
}
