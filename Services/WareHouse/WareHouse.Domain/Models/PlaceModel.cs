

namespace WareHouse.Domain.Models
{
    public class SlotModel
    {
        public Guid PlaceId { get; set; }

        public bool IsFree { get; set; } = true;

        public int MaxWeight { get; set; }
        public string? SKU { get; set; }

        public RackModel OnThisRack { get; set; }
        public Guid OnThisRackId { get; set; }
    }
}
