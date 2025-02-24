using Catalog.Domain.Exceptions;
using Catalog.Domain.Models;
using CommonPractices.CQRS;
using CommonPractices.ResultHandler;
using Marten;
using SlotsService;

namespace Catalog.Application.CQRS.ProductCQRS.Commands
{
    public record UpdateProductBySKU(string sku,string newSKU=null,List<string> category=null, string newDescription=null, decimal price=0):ICommandCP<CustomResult<string>>;
    public class UpdateProdCommHandler(IDocumentSession session, SlotsProtoService.SlotsProtoServiceClient _grpcClient) : ICommHandBase<UpdateProductBySKU, CustomResult<string>>
    {
        public async Task<CustomResult<string>> Handle(UpdateProductBySKU request, CancellationToken cancellationToken)
        {
            Product? existingProduct = session.Query<Product>().FirstOrDefault(prod=>prod.SKU.Equals(request.sku));
            if (existingProduct == null)
            {
                throw new ProductNotFound(request.sku);
            }
            existingProduct.SKU = request.newSKU == null ? existingProduct.SKU : request.newSKU;
            existingProduct.Category = request.category == null ? existingProduct.Category : request.category;
            existingProduct.Description = request.newDescription== null ? existingProduct.Description: request.newDescription;
            existingProduct.Price = request.price== 0 ? existingProduct.Price: request.price;

            if (request.newSKU != null)
            {
                EditSlotBySKUReq req = new EditSlotBySKUReq()
                {
                    SKU = request.sku,
                    NewSKU = request.newSKU
                };
                var r = await _grpcClient.EditSlotBySKUAsync(req);
                if (!r.Succes)
                {
                    return CustomResult<string>.Failure("Unable to update SKU");
                }
            }


            session.Update(existingProduct);
            await session.SaveChangesAsync(cancellationToken);

            return CustomResult<string>.Success(existingProduct.SKU);
        }
    }
}
