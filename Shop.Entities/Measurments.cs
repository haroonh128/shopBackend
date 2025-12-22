namespace Shop.Entities;

public class Measurments : Base
{
    public decimal Length { get; set; }
    public decimal Arm { get; set; }
    public decimal Shoulder { get; set; }
    public decimal Neck { get; set; }
    public decimal Waist { get; set; }
    public decimal Hip { get; set; }
    public decimal Bottom { get; set; }
    public decimal SidePockets { get; set; }
    public decimal FronPocket { get; set; }
    public decimal Trouser { get; set; }
    public string TrouserType { get; set; } = string.Empty;
    public decimal Ankle { get; set; }
    public decimal ArmOpeningType { get; set; }
    public Guid ClientId { get; set; }
}
