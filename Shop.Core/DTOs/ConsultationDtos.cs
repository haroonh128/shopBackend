namespace Shop.Core.DTOs
{
    public class ConsultationListItemResponse
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public string VisitNumber { get; set; } = string.Empty;
        public string? ChiefComplaint { get; set; }
        public string? DiagnosisSummary { get; set; }
        public decimal ConsultationFee { get; set; }
    }

    public class ConsultationVitalsDto
    {
        public decimal? Temperature { get; set; }
        public int? SystolicBP { get; set; }
        public int? DiastolicBP { get; set; }
        public int? Pulse { get; set; }
        public int? RespiratoryRate { get; set; }
        public decimal? OxygenSaturation { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? BMI { get; set; }
    }

    public class ConsultationDiagnosisDto
    {
        public Guid? Id { get; set; }
        public Guid? DiagnosisId { get; set; }
        public string DiagnosisName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public string? Notes { get; set; }
    }

    public class ConsultationTestDto
    {
        public Guid? Id { get; set; }
        public Guid? TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class PrescriptionItemDto
    {
        public Guid? Id { get; set; }
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

    public class PrescriptionDto
    {
        public Guid? Id { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string? Notes { get; set; }
        public List<PrescriptionItemDto> Items { get; set; } = new();
    }

    public class ConsultationDetailsResponse
    {
        public Guid Id { get; set; }
        public Guid PracticeId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string PatientMRNumber { get; set; } = string.Empty;
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
        public ConsultationVitalsDto? Vitals { get; set; }
        public List<ConsultationDiagnosisDto> Diagnoses { get; set; } = new();
        public List<ConsultationTestDto> Tests { get; set; } = new();
        public PrescriptionDto? Prescription { get; set; }
        public DoctorResponse? Doctor { get; set; }
        public PracticeResponse? Practice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ConsultationRequest
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime VisitDate { get; set; }
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
        public ConsultationVitalsDto? Vitals { get; set; }
        public List<ConsultationDiagnosisDto> Diagnoses { get; set; } = new();
        public List<ConsultationTestDto> Tests { get; set; } = new();
        public PrescriptionDto? Prescription { get; set; }
    }
}
