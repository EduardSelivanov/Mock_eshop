using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using SlotsService.Models;
using SlotsService.Repos;
using System.Security.AccessControl;

namespace SlotsService.Services
{
    public partial class SlotService(ISlotRepo _slotRepo) :SlotsProtoService.SlotsProtoServiceBase
    {

       
        public override async Task<MoveProdFromDelResp> MoveProdsFromDel(MoveProdFromDelReq request, ServerCallContext context)
        {
            // FiX FOR WH
            List<SlotModel> slotsToMove = await _slotRepo.GetSlotsByRack(Guid.Parse(request.RackId),true);

            List<SlotModel> FreeSlots = await _slotRepo.GetFreeSlots(true);

            if (FreeSlots.Count < slotsToMove.Count)
            {
                return new MoveProdFromDelResp { Success = false };
            }

            for(int i = 0; i < slotsToMove.Count; i++)
            {
                FreeSlots[i].SKU = slotsToMove[i].SKU;
                FreeSlots[i].ArriveDate = slotsToMove[i].ArriveDate;
                FreeSlots[i].IsFree = false;

            }
            await _slotRepo.RemoveSlots(slotsToMove);
            return new MoveProdFromDelResp() {Success=true};
        }


        

        public override async Task<GetSlotBySKUResp> GetSlotBySKU(GetSlotBySKUReq request, ServerCallContext context)
        {
            // common for Catalog and Ordering
            List<SlotModel> oldest = await _slotRepo.GetSlotsBySKU(request.SKU,request.Count);

            if (oldest == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No such GOOD against this sku"));
            }

            GetSlotBySKUResp resp = new GetSlotBySKUResp()
            {
                SlotId = {oldest.Select(x => x.SlotId.ToString())},
                Place = {oldest.Select(x=>x.Place)},
                Total=oldest.Count()
            };
            return resp;
        }

       

        public override async Task<EmptySlotByIDResp> EmptySlotByID(EmptySlotByIDReq request, ServerCallContext context)
        {
            List<SlotModel> slotsToClear = await _slotRepo.GetSlotsBySKU("asd");

            slotsToClear.ForEach(slot => {
                slot.SKU = null; 
                slot.ArriveDate = null; 
                slot.IsFree = true;
            });

            await _slotRepo.SaveChanges();
            return new EmptySlotByIDResp { Success = true };
        }

       

        public override async Task<ReserveSlotIDResp> ReserveSlotID(ReserveSlotIDReq request, ServerCallContext context)
        {
            SlotModel slotToEdit = await _slotRepo.GetSlotByID(Guid.Parse(request.SlotId));
            //slotToEdit.IsBooked = true;
            //slotToEdit.BookedTill = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
            await _slotRepo.SaveChanges();

            return new ReserveSlotIDResp() {Success=true };
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
