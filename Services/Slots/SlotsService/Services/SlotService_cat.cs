using Grpc.Core;
using SlotsService.Models;

namespace SlotsService.Services
{
    public  partial class SlotService
    {
        public override async Task<AssignProdResp> AssignProductToSlot(AssignProdReq request, ServerCallContext context)
        {
            //Ok
            DateTime dats = request.Date.ToDateTime();
            DateOnly datonl = DateOnly.FromDateTime(dats);
            //SlotModel freeSlot = await _context.SlotsTable.FirstOrDefaultAsync(slot => slot.IsFree);
            SlotModel freeSlot = await _slotRepo.GetFreeSlot();

            if (freeSlot == null)
            {
                return new AssignProdResp { Success = false };
            }
            freeSlot.SKU = request.SKU;
            freeSlot.ArriveDate = datonl;
            freeSlot.IsFree = false;
            await _slotRepo.SaveChanges();

            return new AssignProdResp { Success = true };
        }

        public override async Task<EmptySlotsBySKUResp> EmptySlotsBySKU(EmptySlotsBySKUReq request, ServerCallContext context)
        {
            //Ok
            List<SlotModel> slotsToClear = await _slotRepo.GetSlotsBySKU(request.SKU);

            slotsToClear.ForEach(slot => {
                slot.SKU = null;
                slot.ArriveDate = null;
                slot.IsFree = true;
            });

            await _slotRepo.SaveChanges();
            return new EmptySlotsBySKUResp { Success = true };
        }
        public override async Task<EditSlotBySKUResp> EditSlotBySKU(EditSlotBySKUReq request, ServerCallContext context)
        {
            //OK
            //var slots = await _context.SlotsTable.Where(slot=>slot.SKU.Equals(request.SKU)).ToListAsync();
            var slots = await _slotRepo.GetSlotsBySKU(request.SKU);

            slots.ForEach(slot => slot.SKU = request.NewSKU);
            await _slotRepo.SaveChanges();

            return new EditSlotBySKUResp()
            {
                Success = true
            };
        }
    }
}
