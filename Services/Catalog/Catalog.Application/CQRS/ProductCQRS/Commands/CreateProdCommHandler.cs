using Catalog.Domain.Models;
using CommonPractices.CQRS;
using Marten;
using SlotsService;

namespace Catalog.Application.CQRS.ProductCQRS.Commands
{
    public record CreateProdComm(string sku,List<string> category,string description,decimal price):ICommandCP<string>;

    public class CreateProdCommHandler(IDocumentSession session,SlotsProtoService.SlotsProtoServiceClient _grpcClient) : ICommHandBase<CreateProdComm, string>
    {

        public Task<string> Handle(CreateProdComm request, CancellationToken cancellationToken)
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
                Date= Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.Now)
            };

            session.Store(newProd);
            return null;
        }
    }
}
