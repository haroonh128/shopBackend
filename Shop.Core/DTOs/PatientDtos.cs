using Shop.Common.enums;

namespace Shop.Core.DTOs
{
    public class PatientResponse
    {
        public Guid Id { get; set; }
        public Guid PracticeId { get; set; }
        public string MRNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public int? Age { get; set; }
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
        public DateTime? LastVisitDate { get; set; }
        public string? LastDoctorName { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PatientRequest
    {
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
        public bool Active { get; set; } = true;
        public bool IsDeleted { get; set; }
    }

    public class PatientSearchRequest
    {
        public string? MRNumber { get; set; }
        public string? Name { get; set; }
        public string? ContactNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? RegistrationFrom { get; set; }
        public DateTime? RegistrationTo { get; set; }
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
