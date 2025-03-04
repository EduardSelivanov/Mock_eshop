using Microsoft.EntityFrameworkCore;
using SlotsService.Data;
using SlotsService.Models;

namespace SlotsService.Repos
{
    public class SlotRepo(SlotsContext _context) : ISlotRepo
    {
        public async Task CreateSlot(SlotModel newSlot)
        {
            _context.SlotsTable.Add(newSlot);
            await _context.SaveChangesAsync();
        }
        public async Task CreateSLots(List<SlotModel> newSlots)
        {
            await _context.SlotsTable.AddRangeAsync(newSlots);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SlotModel>> GetSlotsByRack(Guid rackId, bool forEditing = false)
        {
            IQueryable<SlotModel> query = forEditing ? _context.SlotsTable : _context.SlotsTable.AsNoTracking();

            query = query.Where(slot => slot.OnRackId == rackId);

            return await query.ToListAsync();
        }
        public async Task<List<SlotModel>> GetFreeSlots(bool forEditing = false)
        {
            IQueryable<SlotModel> query = forEditing ? _context.SlotsTable : _context.SlotsTable.AsNoTracking();
            query = query.Where(slot => slot.IsFree);
            return await query.ToListAsync();
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
        public async Task RemoveSlots(List<SlotModel> slotsToRemove)
        {
            _context.RemoveRange(slotsToRemove);
        }
        public async Task<SlotModel> GetFreeSlot()
        {
            return await _context.SlotsTable.FirstOrDefaultAsync(slot => slot.IsFree); ;
        }


        public async Task<SlotModel?> GetSlotBySKU(string sku) =>
            await _context.SlotsTable.AsNoTracking()
            .Where(slot => slot.SKU.Equals(sku))
            .OrderBy(slot => slot.ArriveDate)
            .FirstOrDefaultAsync();

        public async Task<SlotModel?> GetSlotByID(Guid slotId)
        {
            return await _context.SlotsTable.FirstOrDefaultAsync(slot => slot.SlotId.Equals(slotId));
        }
        public async Task<List<SlotModel>> GetSlotsById()
        {

            return null;
        }

        public async Task<List<SlotModel>> GetSlotsBySKU(string sku,int quantity=0)
        {
            if (quantity != 0)
            {
                try
                {
                    return await _context.SlotsTable.AsNoTracking()
                     .Where(slot => (slot.SKU.Equals(sku)&&!slot.IsBooked))
                     .OrderBy(slot => slot.ArriveDate)
                     .Take(quantity)
                     .ToListAsync();
                }
                catch
                {
                    return null;
                }
            }
            return  await _context.SlotsTable.Where(slot => slot.SKU.Equals(sku)).ToListAsync();
        }

       
    }
}
