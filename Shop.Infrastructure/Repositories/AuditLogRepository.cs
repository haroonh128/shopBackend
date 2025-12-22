using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<AuditLog>> GetUserActivityAsync(Guid userId, int count = 50)
        {
            return await DbSet
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetEntityHistoryAsync(string entityName, Guid entityId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(a => a.EntityName == entityName)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}
