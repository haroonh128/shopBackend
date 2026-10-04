using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Address>> GetByAddresseeIdAsync(string addresseeId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(a => a.AddresseeId == addresseeId && !a.IsDeleted)
                .ToListAsync();
        }
    }
}