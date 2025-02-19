

namespace Catalog.Domain.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = default!;
        public List<string> Category { get; set; } = new();
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
