using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IPackageRepository : IRepository<Package>
    {
        Task<Package?> GetByCodeAsync(string code);
    }
}