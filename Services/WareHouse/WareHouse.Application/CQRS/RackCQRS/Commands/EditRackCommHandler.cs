using CommonPractices.CQRS;
using MapsterMapper;
using SlotsService;
using WareHouse.Domain.Models;
using WareHouse.Domain.Reopositories;


namespace WareHouse.Application.CQRS.RackCQRS.Commands
{
    
    public record RackDto(string? RackName, string? RackDescription, int RackX=0, int RackY=0);
    public record EditRackComm(Guid rackId,string RackName, string RackDescription, int RackX, int RackY) : ICommandCP<bool>;

    public class EditRackCommHandler(IMapper _mapper, IRackRepo _rackRepo,SlotsProtoService.SlotsProtoServiceClient _grpcClient) : ICommHandBase<EditRackComm, bool>
    {
        public async Task<bool> Handle(EditRackComm request, CancellationToken cancellationToken)
        {
            RackModel assumedRack = await _rackRepo.GetRack(request.rackId,true);

            int diffX = request.RackX-assumedRack.RackX;
            int diffY = request.RackY-assumedRack.RackY;
            int totaldiff = request.RackY * request.RackX - assumedRack.TotalPlaces;

            if (totaldiff > 0)
            {
                IncreaseRackReq increase = new IncreaseRackReq()
                {
                    NewX = request.RackX,
                    NewY = request.RackY,
                    OldX = assumedRack.RackX,
                    OldY=assumedRack.RackY,
                    RackId=request.rackId.ToString()
                };

                IncreaseRackResp resp = await _grpcClient.EditRackWithoutMovingAsync(increase);
                if (!resp.Success)
                {
                    return false;
                }
            }

            assumedRack.RackName = String.IsNullOrWhiteSpace(request.RackName) ? assumedRack.RackName:request.RackName;
            assumedRack.RackDescription = String.IsNullOrWhiteSpace(request.RackDescription)?assumedRack.RackDescription:request.RackDescription;
            assumedRack.RackX = request.RackX is 0?assumedRack.RackX:request.RackX;
            assumedRack.RackY = request.RackY is 0?assumedRack.RackY:request.RackY;

            //assumedRack.TotalPlaces = assumedRack.RackX * assumedRack.RackY;

            if (totaldiff<0)
            {
                EditRackReq moveFrom = new EditRackReq()
                {
                    RackId = request.rackId.ToString(),
                    MaxX=assumedRack.RackX,
                    MaxY=assumedRack.RackY
                };
                EditRackResp resp = await _grpcClient.EditRackAndMoveProdsAsync(moveFrom);

                if (!resp.Succes)
                {
                    return false;
                }
            }
            
            await _rackRepo.EditRack();
            return true;
        }
    }
}
