using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IDiagnosisRepository : IRepository<Diagnosis>
    {
        Task<IEnumerable<Diagnosis>> SearchAsync(Guid practiceId, string? query);
    }
}
