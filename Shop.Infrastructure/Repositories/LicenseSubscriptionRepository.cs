using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class LicenseSubscriptionRepository : Repository<LicenseSubscription>, ILicenseSubscriptionRepository
    {
        public LicenseSubscriptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LicenseSubscription>> GetByLicenseIdAsync(Guid licenseId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(s => s.LicenseId == licenseId && !s.IsDeleted)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<LicenseSubscription?> GetCurrentByLicenseIdAsync(Guid licenseId)
        {
            var now = DateTime.UtcNow;
            return await DbSet
                .AsNoTracking()
                .Where(s => s.LicenseId == licenseId && !s.IsDeleted
                    && s.StartDate <= now && s.EndDate >= now
                    && s.Status == SubscriptionStatus.Active)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();
        }
    }
}