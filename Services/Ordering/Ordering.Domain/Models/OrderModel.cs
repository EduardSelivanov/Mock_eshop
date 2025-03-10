﻿
namespace Ordering.Domain.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public List<string>? Slots { get; set; } = new();
        public List<BasketItem> Basket { get; set;}=new();
        public DateOnly? BookedTill { get; set;}
        public bool? PayedByUser { get; set; } = false;
        public decimal? TotalPrice { get; set;}
    }
    public class BasketItem
    {
        public string Sku { get; set; }
        public int Quantity { get; set;}
    }
}
