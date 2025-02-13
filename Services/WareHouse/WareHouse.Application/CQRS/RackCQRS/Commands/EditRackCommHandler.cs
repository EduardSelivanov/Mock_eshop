
using CommonPractices.CQRS;
using MapsterMapper;
using WareHouse.Domain.Models;
using WareHouse.Domain.Reopositories;


namespace WareHouse.Application.CQRS.RackCQRS.Commands
{
    
    public record RackDto(string? RackName, string? RackDescription, int RackX=0, int RackY=0);
    public record EditRackComm(Guid rackId,string RackName, string RackDescription, int RackX, int RackY) : ICommandCP<bool>;

    public class EditRackCommHandler(IMapper _mapper, IRackRepo _rackRepo) : ICommHandBase<EditRackComm, bool>
    {
        public async Task<bool> Handle(EditRackComm request, CancellationToken cancellationToken)
        {
            RackModel assumedRack = await _rackRepo.GetRack(request.rackId);

            assumedRack.RackName = String.IsNullOrWhiteSpace(request.RackName) ? assumedRack.RackName:request.RackName;
            assumedRack.RackDescription = String.IsNullOrWhiteSpace(request.RackDescription)?assumedRack.RackDescription:request.RackDescription;
            assumedRack.RackX = request.RackX is 0?assumedRack.RackX:request.RackX;
            assumedRack.RackY = request.RackY is 0?assumedRack.RackY:request.RackY;
            assumedRack.TotalPlaces = assumedRack.RackX * assumedRack.RackY;
            await _rackRepo.EditRack();
            return true;
        }
    }
}
