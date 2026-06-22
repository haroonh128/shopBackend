using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class PackageRepository : Repository<Package>, IPackageRepository
    {
        public PackageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Package?> GetByCodeAsync(string code)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted);
        }
    }
}