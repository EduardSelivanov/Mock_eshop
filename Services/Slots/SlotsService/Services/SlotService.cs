using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using SlotsService.Data;
using SlotsService.Models;
using System.Linq;

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
            List<SlotModel> slotsOnThisRack = await _context.SlotsTable.Where(slot => slot.OnRackId.Equals(Guid.Parse(request.RackId))).ToListAsync(); //all slots on this rack

            //need to find slots above max  
            var slotsAboveMAxOnThisRack=slotsOnThisRack.Where(slot => slot.X > request.MaxX || slot.Y > request.MaxY).ToList(); // allslots above max

            // find free slots to delete it from list of empty slots
            var freeSlotsAboveMaxOnThisRack = slotsAboveMAxOnThisRack.Where(slot => slot.IsFree).ToList();

            //find busy slots above
            var busySlotsAboveMaxOnThisRack= slotsAboveMAxOnThisRack.Where(slot => !slot.IsFree).ToList();

            //All free slots and after remove empty slots which are above
            var FreeSlotsTotal = await _context.SlotsTable.Where(slot => slot.IsFree).ToListAsync();
            foreach(var freeSlot in FreeSlotsTotal)
            {
                if (freeSlot.SlotId.Equals(freeSlotsAboveMaxOnThisRack.Any(slot => slot.SlotId.Equals(freeSlot.SlotId))))
                {
                    FreeSlotsTotal.Remove(freeSlot);
                }
            }

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


    //        Google.Protobuf.WellKnownTypes.Timestamp protoTimestamp
    //= Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.Now); //from c# to google
        }


    }
}
