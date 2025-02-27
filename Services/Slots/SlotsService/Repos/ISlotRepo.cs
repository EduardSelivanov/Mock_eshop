using SlotsService.Models;

namespace SlotsService.Repos
{
    public interface ISlotRepo
    {
        Task CreateSlot(SlotModel newSlot);
        Task CreateSLots(List<SlotModel> newSlots);
        Task<SlotModel> GetFreeSlot();
        Task<List<SlotModel>> GetFreeSlots(bool forEditing = false);
        Task<SlotModel?> GetSlotBySKU(string sku);
        Task<SlotModel?> GetSlotByID(Guid slotId);
        Task<List<SlotModel>> GetSlotsByRack(Guid rackId, bool forEditing = false);
        Task<List<SlotModel>> GetSlotsBySKU(string sku,int quantity = 0);
        Task RemoveSlots(List<SlotModel> slotsToRemove);
        Task SaveChanges();
    }
}