
using CommonPractices.CQRS;
using CommonPractices.ResultHandler;
using SlotsService;

namespace Catalog.Application.CQRS.ProductCQRS.Queries
{
    public record GetQuantityBySKU(string sku):ICommandCP<CustomResult<int>>;
    class GetQuantityBySKUQueryHandler(SlotsProtoService.SlotsProtoServiceClient _grpcClient) : ICommHandBase<GetQuantityBySKU, CustomResult<int>>
    {
        public async Task<CustomResult<int>> Handle(GetQuantityBySKU request, CancellationToken cancellationToken)
        {
            var r = await _grpcClient.GetQuntityBySKUAsync(new GetSlotBySKUReq { SKU = request.sku });
            return CustomResult<int>.Success(r.Quntity);
        }
    }
}
