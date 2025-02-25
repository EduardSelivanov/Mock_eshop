using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using SlotsService.Models;
using SlotsService.Repos;

namespace SlotsService.Services
{
    public class SlotService(ISlotRepo _slotRepo) :SlotsProtoService.SlotsProtoServiceBase
    {
        public override async Task<CreateSlotsResp> CreateSlotsForRack(CreateSlotsReq request, ServerCallContext context)
        {
            List<SlotModel> slots = new List<SlotModel>();

            for(int y = 1; y <= request.RackY; y++)
            {
                for(int x = 1; x <= request.RackX; x++)
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
                SlotIds = {slots.Select(s=>s.Place.ToString())}
            };
        }

        public override async Task<IncreaseRackResp> EditRackWithoutMoving(IncreaseRackReq request, ServerCallContext context)
        {
            List<SlotModel> slots = new List<SlotModel>();

            List<SlotModel> existingSlots = await _slotRepo.GetSlotsByRack(Guid.Parse(request.RackId));

            HashSet<(int, int)> existingSlotsHS = existingSlots.Select(slot => (slot.X, slot.Y)).ToHashSet();
           

            for ( int y=1 ; y <= request.NewY; y++)
            {
                for ( int x=1 ; x <= request.NewX; x++)
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
                Succes = true
            }; 
        }

        public override async Task<EditRackResp> EditRackAndMoveProds(EditRackReq request, ServerCallContext context)
        {
            List<SlotModel> slotsOnThisRack = await _slotRepo.GetSlotsByRack(Guid.Parse(request.RackId));//all slots on this rack

            //need to find slots above max  
            List<SlotModel> slotsAboveMAxOnThisRack=slotsOnThisRack
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

            for(int i = 0; i < busySlotsAboveMaxOnThisRack.Count; i++)
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

        public override async Task<MoveProdFromDelResp> MoveProdsFromDel(MoveProdFromDelReq request, ServerCallContext context)
        {
            List<SlotModel> slotsToMove = await _slotRepo.GetSlotsByRack(Guid.Parse(request.RackId),true);

            List<SlotModel> FreeSlots = await _slotRepo.GetFreeSlots(true);

            if (FreeSlots.Count < slotsToMove.Count)
            {
                return new MoveProdFromDelResp { Succes = false };
            }

            for(int i = 0; i < slotsToMove.Count; i++)
            {
                FreeSlots[i].SKU = slotsToMove[i].SKU;
                FreeSlots[i].ArriveDate = slotsToMove[i].ArriveDate;
                FreeSlots[i].IsFree = false;

            }
            await _slotRepo.RemoveSlots(slotsToMove);
            return new MoveProdFromDelResp() {Succes=true};
        }


        public override async Task<AssignProdResp> AssignProductToSlot(AssignProdReq request, ServerCallContext context)
        {
            DateTime dats = request.Date.ToDateTime();
            DateOnly datonl = DateOnly.FromDateTime(dats);
            //SlotModel freeSlot = await _context.SlotsTable.FirstOrDefaultAsync(slot => slot.IsFree);
            SlotModel freeSlot = await _slotRepo.GetFreeSlot();

            if (freeSlot == null)
            {
                return new AssignProdResp { Succes = false };
            }
            freeSlot.SKU = request.SKU;
            freeSlot.ArriveDate = datonl;
            freeSlot.IsFree = false;
            await _slotRepo.SaveChanges();

            return new AssignProdResp { Succes = true };
        }

        public override async Task<GetSlotBySKUResp> GetSlotBySKU(GetSlotBySKUReq request, ServerCallContext context)
        {
            //need to return oldest sku
            SlotModel? oldest = await _slotRepo.GetSlotBySKU(request.SKU);

            if (oldest == null)
            {
                return null;
            }
            GetSlotBySKUResp resp = new GetSlotBySKUResp()
            {
                RackId=oldest.OnRackId.ToString(),
                Place=oldest.Place
            };
            return resp;
        }

        public override async Task<EditSlotBySKUResp> EditSlotBySKU(EditSlotBySKUReq request, ServerCallContext context)
        {
            //var slots = await _context.SlotsTable.Where(slot=>slot.SKU.Equals(request.SKU)).ToListAsync();
            var slots = await _slotRepo.GetSlotsBySKU(request.SKU);

            slots.ForEach(slot => slot.SKU = request.NewSKU);
            await _slotRepo.SaveChanges();

            return new EditSlotBySKUResp()
            {
                Succes = true
            };            
        }

        public override async Task<EmptySlotBySKUResp> EmptySlotBySKU(EmptySlotBySKUReq request, ServerCallContext context)
        {
            List<SlotModel> slotsToClear = await _slotRepo.GetSlotsBySKU(request.SKU);

            slotsToClear.ForEach(slot => {
                slot.SKU = null; 
                slot.ArriveDate = null; 
                slot.IsFree = true;
            });

            await _slotRepo.SaveChanges();
            return new EmptySlotBySKUResp { Succes = true };
        }

        public override async Task<GetQuantityBySKUResp> GetQuntityBySKU(GetSlotBySKUReq request, ServerCallContext context)
        {
            //var slots = await _slotRepo.GetSlotsBySKU(request.SKU);
            var slots = await _slotRepo.GetFreeSlots();
            return new GetQuantityBySKUResp
            {
                Quantity=slots.Count()
            };
            
        }
    }
}
