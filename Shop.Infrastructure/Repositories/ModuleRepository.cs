using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class ModuleRepository : Repository<Modules>, IModuleRepository
    {
        public ModuleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Modules?> GetByCodeAsync(string code)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Code == code && !m.IsDeleted);
        }
    }
}