using Shop.Common.enums;

namespace Shop.Entities;

public class Orders : Base
{
    public string OrderNumber { get; set; } = string.Empty;

    // Shop owner / app subscriber who owns this order
    public Guid UserId { get; set; }

    public Guid ClientId { get; set; }

    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; }

    public string? Description { get; set; }

    public int Quantity { get; set; }

    // Money
    public decimal TotalAmount { get; set; }
    public decimal AdvanceAmount { get; set; }

    // Dates
    public DateTime OrderDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? DeliveryDate { get; set; }

    public string? Notes { get; set; }
}
