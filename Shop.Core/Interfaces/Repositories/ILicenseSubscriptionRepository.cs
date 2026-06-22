using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface ILicenseSubscriptionRepository : IRepository<LicenseSubscription>
    {
        Task<IEnumerable<LicenseSubscription>> GetByLicenseIdAsync(Guid licenseId);
        Task<LicenseSubscription?> GetCurrentByLicenseIdAsync(Guid licenseId);
    }
}