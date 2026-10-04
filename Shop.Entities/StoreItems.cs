namespace Shop.Entities;

public class StoreItems : Base
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal WeighingType { get; set; }
    public DateTime StockDate { get; set; }
    public int Sold { get; set; }
    public decimal Cost { get; set; }
}