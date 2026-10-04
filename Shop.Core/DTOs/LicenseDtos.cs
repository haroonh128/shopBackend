using Shop.Entities;

namespace Shop.Core.DTOs
{
    public class LicenseResponse
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public long ProductId { get; set; }
        public long UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LicenseSubscriptionResponse
    {
        public Guid Id { get; set; }
        public Guid LicenseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = null!;
        public SubscriptionStatus Status { get; set; }
        public Guid? PackageId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateLicenseRequest
    {
        public string Key { get; set; } = string.Empty;
        public long ProductId { get; set; }
        public long UserId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateLicenseRequest
    {
        public bool IsActive { get; set; }
    }

    public class CreateLicenseSubscriptionRequest
    {
        public Guid LicenseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = null!;
        public Guid? PackageId { get; set; }
    }

    public class UpdateLicenseSubscriptionRequest
    {
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public SubscriptionStatus? Status { get; set; }
    }
}
