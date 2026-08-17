using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class CostRepository : Repository<Cost>, ICostRepository
    {
        public CostRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Cost>> GetByClientIdAsync(Guid clientId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(c => c.ClientId == clientId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cost>> GetByMeasurementIdAsync(Guid measurementId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(c => c.MeasurementId == measurementId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
