using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface ILicenseRepository : IRepository<License>
    {
        Task<License?> GetByKeyAsync(string key);
        Task<License?> GetByUserIdAsync(long userId);
        Task<IEnumerable<License>> GetActiveLicensesAsync();
    }
}