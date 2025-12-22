namespace Shop.Entities;

public class Sale : Base
{
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid StoreItemId { get; set; }

}
