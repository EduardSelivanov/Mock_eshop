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
            List<SlotModel> slotsOnThisRack = await _context.SlotsTable.Where(slot => slot.OnRackId.Equals(Guid.Parse(request.RackId))).ToListAsync();

            List<SlotModel> slotsAboveMax = slotsOnThisRack.Where(slot => slot.X > request.MaxX || slot.Y > request.MaxY).ToList();

            List<SlotModel> freeSlotsOnThisRack = slotsOnThisRack.Where(slot => slot.IsFree).ToList();

            List<SlotModel> occupiedSlotsAbove = slotsAboveMax.Where(s => s.IsFree == false).ToList();

            List<SlotModel> FreeSlots = await _context.SlotsTable.Where(s=>s.IsFree).ToListAsync();

            FreeSlots.RemoveAll(s => slotsAboveMax.Any(outof => outof.SlotId == s.SlotId));

            if (FreeSlots.Count < occupiedSlotsAbove.Count)
            {
                return new EditRackResp { Succes = false };
            }

            if (freeSlotsOnThisRack.Count <= occupiedSlotsAbove.Count)
            {
                occupiedSlotsAbove = freeSlotsOnThisRack;
            }

            for(int i = 0; i < occupiedSlotsAbove.Count; i++)
            {
                FreeSlots[i].SKU = occupiedSlotsAbove[i].SKU;
                FreeSlots[i].IsFree = occupiedSlotsAbove[i].IsFree;
                FreeSlots[i].ArriveDate = occupiedSlotsAbove[i].ArriveDate;
            }

            _context.RemoveRange(slotsAboveMax);
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
