using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<IEnumerable<Client>> GetByModuleIdAsync(Guid moduleId);
    }
}
