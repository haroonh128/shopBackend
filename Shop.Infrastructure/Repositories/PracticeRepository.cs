using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class PracticeRepository : Repository<Practice>, IPracticeRepository
    {
        public PracticeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Practice?> GetByUserIdAsync(Guid userId)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
        }
    }
}
