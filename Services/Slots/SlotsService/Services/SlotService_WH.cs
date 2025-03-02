using Grpc.Core;
using SlotsService.Models;

namespace SlotsService.Services
{
    public partial class SlotService
    {
        public override async Task<CreateSlotsResp> CreateSlotsForRack(CreateSlotsReq request, ServerCallContext context)
        {
            //Ok
            List<SlotModel> slots = new List<SlotModel>();

            for (int y = 1; y <= request.RackY; y++)
            {
                for (int x = 1; x <= request.RackX; x++)
                {
                    slots.Add(new SlotModel
                    {
                        SlotId = Guid.NewGuid(),
                        OnRackId = Guid.Parse(request.RackId),
                        SKU = null,
                        X = x,
                        Y = y
                    });
                }
            }
            await _slotRepo.CreateSLots(slots);

            return new CreateSlotsResp
            {
                SlotIds = { slots.Select(s => s.Place.ToString()) }
            };
        }

        public override async Task<IncreaseRackResp> EditRackWithoutMoving(IncreaseRackReq request, ServerCallContext context)
        {
            //OK
            List<SlotModel> slots = new List<SlotModel>();

            List<SlotModel> existingSlots = await _slotRepo.GetSlotsByRack(Guid.Parse(request.RackId));

            HashSet<(int, int)> existingSlotsHS = existingSlots.Select(slot => (slot.X, slot.Y)).ToHashSet();


            for (int y = 1; y <= request.NewY; y++)
            {
                for (int x = 1; x <= request.NewX; x++)
                {
                    if (existingSlotsHS.Contains((x, y)))
                    {
                        continue;
                    }

                    slots.Add(new SlotModel
                    {
                        SlotId = Guid.NewGuid(),
                        OnRackId = Guid.Parse(request.RackId),
                        SKU = null,
                        X = x,
                        Y = y
                    });
                }
            }
            await _slotRepo.CreateSLots(slots);
            await _slotRepo.SaveChanges();
            return new IncreaseRackResp
            {
                Success = true
            };
        }
        public override async Task<EditRackResp> EditRackAndMoveProds(EditRackReq request, ServerCallContext context)
        {
            //Ok
            List<SlotModel> slotsOnThisRack = await _slotRepo.GetSlotsByRack(Guid.Parse(request.RackId));//all slots on this rack

            //need to find slots above max  
            List<SlotModel> slotsAboveMAxOnThisRack = slotsOnThisRack
                .Where(slot => slot.X > request.MaxX || slot.Y > request.MaxY).ToList(); // allslots above max

            // find free slots to delete it from list of empty slots
            List<SlotModel> freeSlotsAboveMaxOnThisRack = slotsAboveMAxOnThisRack
                .Where(slot => slot.IsFree).ToList();

            //find busy slots above
            List<SlotModel> busySlotsAboveMaxOnThisRack = slotsAboveMAxOnThisRack
                .Where(slot => !slot.IsFree).ToList();

            //All free slots and after remove empty slots which are above
            List<SlotModel> FreeSlotsTotal = await _slotRepo.GetFreeSlots(true);

            FreeSlotsTotal.RemoveAll(freeSlot => freeSlotsAboveMaxOnThisRack
                .Select(slot => slot.SlotId)
                .Contains(freeSlot.SlotId));

            //if not enough space just refuse request
            if (FreeSlotsTotal.Count < busySlotsAboveMaxOnThisRack.Count)
            {
                return new EditRackResp { Succes = false };
            }

            for (int i = 0; i < busySlotsAboveMaxOnThisRack.Count; i++)
            {
                FreeSlotsTotal[i].SKU = busySlotsAboveMaxOnThisRack[i].SKU;
                FreeSlotsTotal[i].IsFree = busySlotsAboveMaxOnThisRack[i].IsFree;
                FreeSlotsTotal[i].ArriveDate = busySlotsAboveMaxOnThisRack[i].ArriveDate;
            }

            await _slotRepo.RemoveSlots(busySlotsAboveMaxOnThisRack);
            await _slotRepo.RemoveSlots(freeSlotsAboveMaxOnThisRack);
            await _slotRepo.SaveChanges();

            return new EditRackResp { Succes = true };
        }
    }
}
