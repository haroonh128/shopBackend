namespace Shop.Entities
{
    public class Consultation : Base
    {
        public Guid PracticeId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime VisitDate { get; set; }
        public string VisitNumber { get; set; } = string.Empty;
        public string? ChiefComplaint { get; set; }
        public string? History { get; set; }
        public string? Examination { get; set; }
        public string? Observations { get; set; }
        public string? DiagnosisNotes { get; set; }
        public string? Advice { get; set; }
        public bool FollowUpRequired { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string? FollowUpNotes { get; set; }
        public decimal ConsultationFee { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? ModifiedByUserId { get; set; }
    }
}
