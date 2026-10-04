using Shop.Common.enums;

namespace Shop.Entities
{
    public class StoreItem : Base
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public UOM Unit { get; set; }
        public bool IsActive { get; set; }

        public Guid ShopId { get; set; }
    }
}
