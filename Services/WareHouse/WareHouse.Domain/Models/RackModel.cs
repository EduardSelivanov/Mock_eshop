
namespace WareHouse.Domain.Models
{
    public class RackModel
    {
        public Guid RackId { get; set; }

        public string RackName { get; set; }

        public string RackDescription { get; set; }

        public int RackX { get; set; } //assume rack is rectangle, so X max spaces per floor
        public int RackY { get; set; } // Y floor 
        public int TotalPlaces { get; set; }

    }
}
