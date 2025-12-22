using Shop.Common.enums;

namespace Shop.Entities;

public class Transaction : Base
{
    public Guid SaleId { get; set; }
    public string Matadata { get; set; } = string.Empty;
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime TransactionDate { get; set; }

}
