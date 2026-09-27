namespace Shop.Entities
{
    public class PrescriptionItem : Base
    {
        public Guid PrescriptionId { get; set; }
        public Guid? MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string? Strength { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Route { get; set; }
        public string? Duration { get; set; }
        public string? Quantity { get; set; }
        public string? Instructions { get; set; }
        public int SortOrder { get; set; }
    }
}
