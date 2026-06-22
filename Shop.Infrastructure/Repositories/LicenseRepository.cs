using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class LicenseRepository : Repository<License>, ILicenseRepository
    {
        public LicenseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<License?> GetByKeyAsync(string key)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Key == key && !l.IsDeleted);
        }

        public async Task<License?> GetByUserIdAsync(long userId)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.UserId == userId && !l.IsDeleted);
        }

        public async Task<IEnumerable<License>> GetActiveLicensesAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(l => l.IsActive && !l.IsDeleted)
                .ToListAsync();
        }
    }
}