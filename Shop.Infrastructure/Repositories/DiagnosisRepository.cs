using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class DiagnosisRepository : Repository<Diagnosis>, IDiagnosisRepository
    {
        public DiagnosisRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Diagnosis>> SearchAsync(Guid practiceId, string? query)
        {
            var q = DbSet.AsNoTracking().Where(d => d.PracticeId == practiceId && !d.IsDeleted && d.Active);
            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(d => d.Name.Contains(query.Trim()));
            return await q.OrderBy(d => d.Name).Take(50).ToListAsync();
        }
    }
}
