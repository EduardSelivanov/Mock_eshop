

using Catalog.Domain.Exceptions;
using Catalog.Domain.Models;
using CommonPractices.CQRS;
using CommonPractices.ResultHandler;
using Marten;
using SlotsService;

namespace Catalog.Application.CQRS.ProductCQRS.Commands
{
    public record DeleteProdBySKU(string sku):ICommandCP<CustomResult<bool>>;
    class DeleteProdCommHandler(IDocumentSession session,SlotsProtoService.SlotsProtoServiceClient _grpc) : ICommHandBase<DeleteProdBySKU, CustomResult<bool>>
    {
        public async Task<CustomResult<bool>> Handle(DeleteProdBySKU request, CancellationToken cancellationToken)
        {
            Product? prodToDel = await session.Query<Product>().FirstOrDefaultAsync(prod=>prod.SKU.Equals(request.sku));
            if (prodToDel == null)
            {
                throw new ProductNotFound(request.sku);
            }
            EmptySlotsBySKUReq req = new EmptySlotsBySKUReq{ SKU=request.sku};
            EmptySlotsBySKUResp resp = await _grpc.EmptySlotsBySKUAsync(req);
            if (!resp.Success)
            {
                CustomResult<bool>.Failure("Some error in GRPC");
            }
            session.Delete(prodToDel);
            await session.SaveChangesAsync();

            return  CustomResult<bool>.Success(true);
        }
    }
}
