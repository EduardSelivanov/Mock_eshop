namespace SlotsService.Models
{
    public class SlotModel
    {
        public Guid SlotId { get; set;}
        public Guid OnRackId { get; set;}
        public string? SKU { get; set; }
        public DateOnly? ArriveDate { get; set; }
        public bool IsBooked { get; set; } = false;
        public bool IsFree { get; set; } = true;

        public int X { get; set; } // by width
        public int Y { get; set; } // height

        public string Place => $"x={X},y={Y}";
    }
}
