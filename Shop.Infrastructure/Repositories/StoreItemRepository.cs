using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class StoreItemRepository : Repository<StoreItem>, IStoreItemRepository
    {
        public StoreItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StoreItem>> GetByShopIdAsync(Guid shopId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(s => s.ShopId == shopId && !s.IsDeleted)
                .ToListAsync();
        }
    }
}
