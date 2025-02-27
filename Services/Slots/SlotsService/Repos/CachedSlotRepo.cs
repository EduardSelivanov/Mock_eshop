using Microsoft.Extensions.Caching.Distributed;
using SlotsService.Models;
using System.Text.Json;

namespace SlotsService.Repos
{
    public class CachedSlotRepo : ISlotRepo
    {
        private ISlotRepo _repo;
        private IDistributedCache _cache;
        private DistributedCacheEntryOptions _redisOpts;

        public CachedSlotRepo(ISlotRepo repo, IDistributedCache cache)
        {
            _cache = cache;
            _repo = repo;
            _redisOpts = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(8)};
        }

        public async Task<List<SlotModel>> GetFreeSlots(bool forEditing = false)
        {
            string cachedFreeSlots = await _cache.GetStringAsync("FreeSlots");
            if (!string.IsNullOrEmpty(cachedFreeSlots))
            {
                return JsonSerializer.Deserialize<List<SlotModel>>(cachedFreeSlots);
            }
            List<SlotModel> freeSlots = await _repo.GetFreeSlots();
            await _cache.SetStringAsync("FreeSlots",JsonSerializer.Serialize(freeSlots),_redisOpts);

            return freeSlots;
        }

        public async Task<List<SlotModel>> GetSlotsByRack(Guid rackId, bool forEditing = false)
        {
            string cachedSlotsOnThisRack = await _cache.GetStringAsync($"{rackId.ToString()}");
            if (!string.IsNullOrEmpty(cachedSlotsOnThisRack))
            {
                return JsonSerializer.Deserialize < List<SlotModel>>(cachedSlotsOnThisRack);
            }

            List<SlotModel> slotsOnThisRack = await _repo.GetSlotsByRack(rackId, forEditing);
            await _cache.SetStringAsync($"{rackId.ToString()}", JsonSerializer.Serialize(slotsOnThisRack), _redisOpts);
            return slotsOnThisRack;
        }

        public async Task SaveChanges()
        {
            await _repo.SaveChanges();
            await _cache.RemoveAsync("FreeSlots");
        }
        public async Task RemoveSlots(List<SlotModel> slotsToRemove)
        {
            await _repo.RemoveSlots(slotsToRemove);
            await _cache.RemoveAsync("FreeSlots");
        }


        public async Task CreateSlot(SlotModel newSlot)
        {
            await _repo.CreateSlot(newSlot);
        }

        public async Task CreateSLots(List<SlotModel> newSlots)
        {
            await _repo.CreateSLots(newSlots);
        }

        public async Task<SlotModel> GetFreeSlot()
        {
            return await _repo.GetFreeSlot();
        }

        public async Task<SlotModel?> GetSlotByID(Guid slotId)
        {
            return await _repo.GetSlotByID(slotId);
        }

        public async Task<SlotModel?> GetSlotBySKU(string sku)
        {
           
            return await _repo.GetSlotBySKU(sku);
        }

        public async Task<List<SlotModel>> GetSlotsBySKU(string sku,int p=0)
        {
            return await _repo.GetSlotsBySKU(sku,p);
        }
    }
}
