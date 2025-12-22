using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{


    public class OtpRepository : Repository<OtpCode>, IOtpRepository
    {
        public OtpRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<OtpCode?> GetValidOtpAsync(Guid userId, string code)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(o =>
                    o.UserId == userId &&
                    o.Code == code &&
                    !o.IsUsed &&
                    o.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<List<OtpCode>> GetExpiredOtpsAsync(DateTime cutoffTime)
        {
            return await DbSet
                .Where(o => o.ExpiresAt < cutoffTime && !o.IsUsed)
                .ToListAsync();
        }

        public async Task<int> GetActiveOtpCountAsync(Guid userId)
        {
            return await DbSet
                .CountAsync(o =>
                    o.UserId == userId &&
                    !o.IsUsed &&
                    o.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<List<OtpCode>> GetUserOtpHistoryAsync(Guid userId, int count = 10)
        {
            return await DbSet
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}

