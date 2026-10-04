using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class TestRepository : Repository<Test>, ITestRepository
    {
        public TestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Test>> SearchAsync(Guid practiceId, string? query)
        {
            var q = DbSet.AsNoTracking().Where(t => t.PracticeId == practiceId && !t.IsDeleted && t.Active);
            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(t => t.Name.Contains(query.Trim()));
            return await q.OrderBy(t => t.Name).Take(50).ToListAsync();
        }
    }
}
