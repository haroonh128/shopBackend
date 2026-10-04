using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IMedicationRepository : IRepository<Medication>
    {
        Task<IEnumerable<Medication>> SearchAsync(Guid practiceId, string? query);
    }
}
