using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetValidTokenAsync(string token)
        {
            return await DbSet
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == token &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            var tokens = await DbSet
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.UpdatedAt = DateTime.UtcNow;
            }
        }

        public async Task<List<RefreshToken>> GetActiveUserTokensAsync(Guid userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(rt =>
                    rt.UserId == userId &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetActiveTokenCountAsync(Guid userId)
        {
            return await DbSet
                .CountAsync(rt =>
                    rt.UserId == userId &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);
        }
    }
}
