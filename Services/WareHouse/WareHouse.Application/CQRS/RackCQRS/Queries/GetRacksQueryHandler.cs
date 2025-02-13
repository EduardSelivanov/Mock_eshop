
using CommonPractices.CQRS;
using WareHouse.Domain.Models;
using WareHouse.Domain.Reopositories;

namespace WareHouse.Application.CQRS.RackCQRS.Queries
{
    public record GetRacksQuery(int page=1,int pageSize=10):IQueryCP<IEnumerable<RackModel>>;

    public class GetRacksQueryHandler(IRackRepo _rackRepo) : IQueryHandlerBase<GetRacksQuery, IEnumerable<RackModel>>
    {
        public async Task<IEnumerable<RackModel>> Handle(GetRacksQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<RackModel> racks = await _rackRepo.GetRacks(request.page ,request.pageSize);
            return racks;
        }
    }
}
