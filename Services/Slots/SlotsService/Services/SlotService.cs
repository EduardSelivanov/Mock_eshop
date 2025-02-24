using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using SlotsService.Data;
using SlotsService.Models;

namespace SlotsService.Services
{
    public class SlotService(SlotsContext _context) :SlotsProtoService.SlotsProtoServiceBase
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
            _context.SlotsTable.AddRange(slots);
            await _context.SaveChangesAsync();

            return new CreateSlotsResp
            {
                SlotIds = {slots.Select(s=>s.Place.ToString())}
            };
        }

        public override async Task<IncreaseRackResp> EditRackWithoutMoving(IncreaseRackReq request, ServerCallContext context)
        {
            List<SlotModel> slots = new List<SlotModel>();

            List<SlotModel> existingSlots = await _context.SlotsTable
                .Where(slot => slot.OnRackId == Guid.Parse(request.RackId)).ToListAsync();

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
            _context.SlotsTable.AddRange(slots);
            await _context.SaveChangesAsync();

            return new IncreaseRackResp
            {
                Succes = true
            };
        }

        public override async Task<EditRackResp> EditRackAndMoveProds(EditRackReq request, ServerCallContext context)
        {
            List<SlotModel> slotsOnThisRack = await _context.SlotsTable
                .Where(slot => slot.OnRackId.Equals(Guid.Parse(request.RackId)))
                .ToListAsync(); //all slots on this rack

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
            List<SlotModel> FreeSlotsTotal = await _context.SlotsTable
                 .Where(slot => slot.IsFree)
                 .ToListAsync();

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

            _context.RemoveRange(busySlotsAboveMaxOnThisRack);
            _context.RemoveRange(freeSlotsAboveMaxOnThisRack);
            await _context.SaveChangesAsync();

            return new EditRackResp { Succes = true };
        }

        public override async Task<MoveProdFromDelResp> MoveProdsFromDel(MoveProdFromDelReq request, ServerCallContext context)
        {
            List<SlotModel> slotsToMove = await _context.SlotsTable
                .Where(s=>s.OnRackId.Equals(Guid.Parse(request.RackId))
                && s.SKU!=null).ToListAsync();

            List<SlotModel> FreeSlots = await _context.SlotsTable.Where(s => s.IsFree).ToListAsync();

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

            _context.SlotsTable.RemoveRange(slotsToMove);
            await _context.SaveChangesAsync();

            return new MoveProdFromDelResp() {Succes=true};
        }


        public override async Task<AssignProdResp> AssignProductToSlot(AssignProdReq request, ServerCallContext context)
        {
            DateTime dats = request.Date.ToDateTime();
            DateOnly datonl = DateOnly.FromDateTime(dats);
            SlotModel freeSlot = await _context.SlotsTable.FirstOrDefaultAsync(slot => slot.IsFree);

            if (freeSlot == null)
            {
                return new AssignProdResp { Succes = false };
            }
            freeSlot.SKU = request.SKU;
            freeSlot.ArriveDate = datonl;
            freeSlot.IsFree = false;

            await _context.SaveChangesAsync();

            return new AssignProdResp { Succes = true };
        }

        public override async Task<GetSlotBySKUResp> GetSlotBySKU(GetSlotBySKUReq request, ServerCallContext context)
        {
            //need to return oldest sku
            SlotModel? oldest =await _context.SlotsTable
                .AsNoTracking()
                .Where(slot=>slot.SKU.Equals(request.SKU))
                .OrderBy(slot=>slot.ArriveDate)
                .FirstOrDefaultAsync();

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
            var slots = await _context.SlotsTable.Where(slot=>slot.SKU.Equals(request.SKU)).ToListAsync();
            slots.ForEach(slot => slot.SKU = request.NewSKU);
            await _context.SaveChangesAsync();

            return new EditSlotBySKUResp()
            {
                Succes = true
            };            
        }

        public override async Task<EmptySlotBySKUResp> EmptySlotBySKU(EmptySlotBySKUReq request, ServerCallContext context)
        {
            List<SlotModel> slotsToClear = await _context.SlotsTable.Where(slot => slot.SKU.Equals(request.SKU)).ToListAsync();

            slotsToClear.ForEach(slot => {
                slot.SKU = null; 
                slot.ArriveDate = null; 
                slot.IsFree = true;
            });

            await _context.SaveChangesAsync();
            return new EmptySlotBySKUResp { Succes = true };
        }
    }
}
