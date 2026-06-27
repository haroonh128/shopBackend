using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IMeasurementRepository : IRepository<Measurements>
    {
        Task<IEnumerable<Measurements>> GetByClientIdAsync(Guid clientId);
    }
}
