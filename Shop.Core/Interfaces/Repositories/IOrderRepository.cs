using Shop.Common.enums;
using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Orders>
    {
        Task<IEnumerable<Orders>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Orders>> GetByClientIdAsync(Guid userId, Guid clientId);
        Task<IEnumerable<Orders>> GetByStatusAsync(Guid userId, OrderStatus status);
        Task<Orders?> GetByOrderNumberAsync(string orderNumber);
    }
}
