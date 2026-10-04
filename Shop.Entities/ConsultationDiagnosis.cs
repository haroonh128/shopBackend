namespace Shop.Entities
{
    public class ConsultationDiagnosis : Base
    {
        public Guid ConsultationId { get; set; }
        public Guid? DiagnosisId { get; set; }
        public string DiagnosisName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public string? Notes { get; set; }
    }
}
