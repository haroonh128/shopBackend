using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class MedicationRepository : Repository<Medication>, IMedicationRepository
    {
        public MedicationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Medication>> SearchAsync(Guid practiceId, string? query)
        {
            var q = DbSet.AsNoTracking().Where(m => m.PracticeId == practiceId && !m.IsDeleted && m.Active);
            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(m => m.Name.Contains(query.Trim()) || (m.GenericName != null && m.GenericName.Contains(query.Trim())));
            return await q.OrderBy(m => m.Name).Take(50).ToListAsync();
        }
    }
}
