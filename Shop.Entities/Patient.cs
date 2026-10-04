using Shop.Common.enums;

namespace Shop.Entities
{
    public class Patient : Base
    {
        public Guid PracticeId { get; set; }
        public string MRNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string? AlternateContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? Allergies { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? BMI { get; set; }
        public string? Notes { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? ModifiedByUserId { get; set; }
    }
}
