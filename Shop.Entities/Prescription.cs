namespace Shop.Entities
{
    public class Prescription : Base
    {
        public Guid ConsultationId { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string? Notes { get; set; }
    }
}
