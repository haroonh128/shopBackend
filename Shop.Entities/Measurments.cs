using Shop.Common.enums;

namespace Shop.Entities;

public class Measurements : Base
{
    public Guid ClientId { get; set; }

    // Garment Type
    public MeasurementType Type { get; set; }

    // Upper Body
    public decimal? Length { get; set; }
    public decimal? Shoulder { get; set; }
    public decimal? Chest { get; set; }
    public decimal? Waist { get; set; }
    public decimal? Hip { get; set; }
    public decimal? Neck { get; set; }

    // Sleeves
    public decimal? ArmLength { get; set; }
    public decimal? ArmRound { get; set; }
    public decimal? Bicep { get; set; }
    public decimal? Elbow { get; set; }
    public decimal? Wrist { get; set; }
    public decimal? ArmOpening { get; set; }

    // Lower Body
    public decimal? TrouserLength { get; set; }
    public decimal? Inseam { get; set; }
    public decimal? Rise { get; set; }
    public decimal? Thigh { get; set; }
    public decimal? Knee { get; set; }
    public decimal? Calf { get; set; }
    public decimal? Ankle { get; set; }
    public decimal? Bottom { get; set; }

    // Special Measurements
    public decimal? UnderBust { get; set; }
    public decimal? Bust { get; set; }
    public decimal? NeckDepthFront { get; set; }
    public decimal? NeckDepthBack { get; set; }

    // Pockets
    public bool? SidePocket { get; set; }
    public bool? FrontPocket { get; set; }
    public bool? BackPocket { get; set; }
    public bool? BreastPocket { get; set; }
    public bool? InnerPocket { get; set; }
    public bool? PatchPocket { get; set; }
    public bool? TicketPocket { get; set; }

    // Style Options
    public string? CollarType { get; set; }
    public string? CuffType { get; set; }
    public string? FitType { get; set; }
    public string? TrouserType { get; set; }
    public string? LapelStyle { get; set; }
    public string? NeckStyle { get; set; }
    public string? ArmType { get; set; }
    public string? VentType { get; set; }
    public string? WaistcoatType { get; set; }
    public string? RiseType { get; set; }
    public string? HemType { get; set; }
    public string? FrontPocketType { get; set; }
    public string? BackPocketType { get; set; }
    public bool? ElbowPatch { get; set; }

    // Misc
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }

    public string? Notes { get; set; }
}
