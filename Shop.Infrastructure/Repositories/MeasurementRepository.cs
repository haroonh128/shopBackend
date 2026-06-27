using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class MeasurementRepository : Repository<Measurements>, IMeasurementRepository
    {
        public MeasurementRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Measurements>> GetByClientIdAsync(Guid clientId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(m => m.ClientId == clientId && !m.IsDeleted)
                .ToListAsync();
        }
    }
}
