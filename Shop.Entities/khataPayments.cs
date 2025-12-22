namespace Shop.Entities;

public class khataPayments : Base
{
    public Guid KhataId { get; set; }
    public decimal Amount { get; set; }
    public Guid TransactionId { get; set; }
}
