using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IPracticeRepository : IRepository<Practice>
    {
        Task<Practice?> GetByUserIdAsync(Guid userId);
    }
}
