

using WareHouse.Domain.Models;

namespace WareHouse.Domain.Reopositories
{
    public interface IRackRepo
    {
        Task<Guid> CreateRack(RackModel newRack);
        Task<IEnumerable<RackModel>> GetRacks(int page = 1, int pagesize = 10);
        Task<bool> EditRack();
        Task<bool> DeleteRack(RackModel rackToDelete);
        Task<RackModel> GetRack(Guid rackId,bool forEditing=false);
    }
}
