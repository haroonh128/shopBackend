using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IPackageModuleRepository : IRepository<PackageModule>
    {
        Task<IEnumerable<PackageModule>> GetByPackageIdAsync(Guid packageId);
        Task<IEnumerable<PackageModule>> GetByModuleIdAsync(Guid moduleId);
    }
}