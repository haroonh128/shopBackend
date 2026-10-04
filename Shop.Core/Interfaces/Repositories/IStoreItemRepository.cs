using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IStoreItemRepository : IRepository<StoreItem>
    {
        Task<IEnumerable<StoreItem>> GetByShopIdAsync(Guid shopId);
    }
}