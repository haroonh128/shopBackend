using Shop.Common.enums;

namespace Shop.Entities;

public class Khata : Base
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Cnic { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public decimal Amount { get; set; }
}
