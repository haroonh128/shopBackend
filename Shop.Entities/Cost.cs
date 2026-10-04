namespace Shop.Entities;

public class Cost : Base
{
    public Guid ClientId { get; set; }
    public Guid? MeasurementId { get; set; }
    public decimal TotalCost { get; set; }
    public decimal Advance { get; set; }
    public string? Notes { get; set; }
}
