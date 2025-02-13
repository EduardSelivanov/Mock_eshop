
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using WareHouse.Domain.Models;
using WareHouse.Domain.Reopositories;
using WareHouse.Domain.WareHouseExceptions;
using WareHouse.Infrastructure.DataBase;

namespace WareHouse.Infrastructure.Repos
{
    internal class RackRepo(WareHouseContext _context): IRackRepo
    {
        public async Task<Guid> CreateRack(RackModel newRack)
        {
            RackModel assumedRack = await RetriveRackByName(newRack.RackName);
            if (assumedRack != null)
            {
                throw new Exception();
            }
            await _context.RackTable.AddAsync(newRack);
            await _context.SaveChangesAsync();

            return newRack.RackId;
        }

        public async Task<RackModel> GetRack(Guid rackId)
        {
            RackModel assumedRack = await RetriveRackbyId(rackId);
            if (assumedRack is null)
            {
                throw new RackNotFoundException(rackId); 
            }
            return assumedRack;
        }
        public async Task<IEnumerable<RackModel>> GetRacks(int page=1, int pagesize=10)
        {
            IEnumerable<RackModel> racks = await _context.RackTable
                .Skip((page-1) * pagesize)
                .Take(pagesize)
                .ToListAsync();
            
            return racks;
        }
        public async Task<bool> DeleteRack(RackModel rackToDelete)
        {
            _context.RackTable.Remove(rackToDelete);
            await _context.SaveChangesAsync();
            return true;    
        }
        public async Task<bool> EditRack()
        {
            await _context.SaveChangesAsync();
            return true;
        }
        private async Task<RackModel> RetriveRackbyId(Guid id)
        {
            RackModel asumedRack = await _context.RackTable.FirstOrDefaultAsync(rack => rack.RackId.Equals(id));
            if (asumedRack == null)
            {
                return null;
            }
            return asumedRack;
        }
        private async Task<RackModel> RetriveRackByName(string name)
        {
            RackModel? asumedRack = await _context.RackTable.FirstOrDefaultAsync(
                rack =>rack.RackName.ToLower().Replace(" ", "").Equals(name.ToLower().Replace(" ","")));
            if (asumedRack == null)
            {
                return null;
            }
            return asumedRack;
        }
        public async Task<bool> EditRack(RackModel rackToEdit)
        {
            return false;
        }
    }
}
