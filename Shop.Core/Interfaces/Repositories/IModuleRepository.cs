using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IModuleRepository : IRepository<Modules>
    {
        Task<Modules?> GetByCodeAsync(string code);
    }
}