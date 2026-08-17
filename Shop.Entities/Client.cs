using Shop.Common.enums;

namespace Shop.Entities;

public class Client : Base
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public Guid ModuleId { get; set; }
}
