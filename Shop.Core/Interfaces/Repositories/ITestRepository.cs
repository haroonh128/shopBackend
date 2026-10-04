using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface ITestRepository : IRepository<Test>
    {
        Task<IEnumerable<Test>> SearchAsync(Guid practiceId, string? query);
    }
}
