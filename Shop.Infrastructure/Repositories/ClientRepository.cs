using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Client>> GetByUserIdAsync(Guid userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetByModuleIdAsync(Guid userId, Guid moduleId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(c => c.UserId == userId && c.ModuleId == moduleId && !c.IsDeleted)
                .ToListAsync();
        }
    }
}
