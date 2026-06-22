using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetByRegistrationNumberAsync(string registrationNumber)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.RegistrationNumber == registrationNumber && !p.IsDeleted);
        }
    }
}