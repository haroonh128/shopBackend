namespace Shop.Core.DTOs
{
    public class StoreItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public int Unit { get; set; } // UOM enum value (e.g. 1=Piece, 2=Meter)
        public bool IsActive { get; set; }
        public Guid ShopId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateStoreItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public int Unit { get; set; } // UOM enum value
        public bool IsActive { get; set; } = true;
        public Guid ShopId { get; set; }
    }

    public class UpdateStoreItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public int Unit { get; set; }
        public bool IsActive { get; set; }
    }
}
