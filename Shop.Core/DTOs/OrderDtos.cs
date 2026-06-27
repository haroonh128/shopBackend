using Shop.Common.enums;

namespace Shop.Core.DTOs
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateOrderRequest
    {
        public Guid ClientId { get; set; }
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? Description { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal TotalAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateOrderRequest
    {
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
    }
}
