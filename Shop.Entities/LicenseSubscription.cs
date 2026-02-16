namespace Shop.Entities
{
    public class LicenseSubscription: Base
    {
        public Guid LicenseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = null!;
        public SubscriptionStatus Status { get; set; }
        public Guid? PackageId { get; set; }
    }
}
