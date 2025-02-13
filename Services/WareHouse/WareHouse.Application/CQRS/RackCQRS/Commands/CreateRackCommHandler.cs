using CommonPractices.CQRS;
using Mapster;
using MapsterMapper;
using WareHouse.Domain.Models;
using WareHouse.Domain.Reopositories;

namespace WareHouse.Application.CQRS.RackCQRS.Commands
{
    public record CreateRackComm(string RackName, string RackDescription, int RackX, int RackY) : ICommandCP<string>;

    public class CreateRackCommHandler(IRackRepo _rackrepo) : ICommHandBase<CreateRackComm, string>
    {

        public async Task<string> Handle(CreateRackComm request, CancellationToken cancellationToken)
        {
            RackModel newRack = new RackModel();


            newRack = new RackModel()
            {
                RackId = Guid.NewGuid(),
                RackDescription = request.RackDescription,
                RackName = request.RackName,
                RackX = request.RackX,
                RackY = request.RackY
            };

            var res = await _rackrepo.CreateRack(newRack);

            var d = newRack.Adapt<CreateRackComm>();

            return d.RackName;
        }

    }
}
