namespace Shop.Entities;

public class Expenses: Base
{
    public string Category { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Amount { get; set; }

}
