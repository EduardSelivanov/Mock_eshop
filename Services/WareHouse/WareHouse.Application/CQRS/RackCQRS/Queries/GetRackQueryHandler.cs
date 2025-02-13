using CommonPractices.CQRS;
using WareHouse.Domain.Models;
using WareHouse.Domain.Reopositories;

namespace WareHouse.Application.CQRS.RackCQRS.Queries
{
    public record GetRackByIdQuery(Guid id):IQueryCP<RackModel>;
    internal class GetRackQueryHandler(IRackRepo _rackRepo) : IQueryHandlerBase<GetRackByIdQuery, RackModel>
    {
        public async Task<RackModel> Handle(GetRackByIdQuery request, CancellationToken cancellationToken)
        {
            RackModel rack = await _rackRepo.GetRack(request.id);
            await Task.Delay(5000);


            return rack; 
        }
    }
}
