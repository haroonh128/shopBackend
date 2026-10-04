using Microsoft.EntityFrameworkCore;
using Shop.Common.enums;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Orders>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Orders>> GetByUserIdAsync(Guid userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Orders>> GetByClientIdAsync(Guid userId, Guid clientId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.ClientId == clientId && !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Orders>> GetByStatusAsync(Guid userId, OrderStatus status)
        {
            return await DbSet
                .AsNoTracking()
                .Where(o => o.UserId == userId && o.Status == status && !o.IsDeleted)
                .ToListAsync();
        }

        public async Task<Orders?> GetByOrderNumberAsync(string orderNumber)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && !o.IsDeleted);
        }
    }
}
