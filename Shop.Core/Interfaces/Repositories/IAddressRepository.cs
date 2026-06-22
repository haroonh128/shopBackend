using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> GetByAddresseeIdAsync(string addresseeId);
    }
}