
using CommonPractices.CQRS;
using WareHouse.Domain.Models;
using WareHouse.Domain.Reopositories;

namespace WareHouse.Application.CQRS.RackCQRS.Commands
{
    public record DeleteRackComm(Guid rackId):ICommandCP<string>;
    internal class DeleteRackCommHandler(IRackRepo _rackRepo) : ICommHandBase<DeleteRackComm, string>
    {
        public async Task<string> Handle(DeleteRackComm request, CancellationToken cancellationToken)
        {
            RackModel rackToDelete = await _rackRepo.GetRack(request.rackId);

            var deleted = await _rackRepo.DeleteRack(rackToDelete);
            return rackToDelete.RackName;
        }
    }
}
