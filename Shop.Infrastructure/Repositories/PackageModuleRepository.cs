using Microsoft.EntityFrameworkCore;
using Shop.Entities;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class PackageModuleRepository : Repository<PackageModule>, IPackageModuleRepository
    {
        public PackageModuleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PackageModule>> GetByPackageIdAsync(Guid packageId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(pm => pm.Package)
                .Include(pm => pm.Module)
                .Where(pm => pm.PackageId == packageId && !pm.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<PackageModule>> GetByModuleIdAsync(Guid moduleId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(pm => pm.Package)
                .Include(pm => pm.Module)
                .Where(pm => pm.ModuleId == moduleId && !pm.IsDeleted)
                .ToListAsync();
        }
    }
}