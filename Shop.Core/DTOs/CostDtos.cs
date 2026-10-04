namespace Shop.Core.DTOs
{
    public class CostResponse
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid? MeasurementId { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Advance { get; set; }
        public decimal Pending { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCostRequest
    {
        public Guid ClientId { get; set; }
        public Guid? MeasurementId { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Advance { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateCostRequest : CreateCostRequest
    {
    }
}
