using Shop.Core.Interfaces;
using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface ICostRepository : IRepository<Cost>
    {
        Task<IEnumerable<Cost>> GetByClientIdAsync(Guid clientId);
        Task<IEnumerable<Cost>> GetByMeasurementIdAsync(Guid measurementId);
    }
}
