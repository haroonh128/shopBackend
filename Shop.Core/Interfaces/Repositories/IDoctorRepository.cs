using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetByPracticeIdAsync(Guid practiceId);
        Task<Doctor?> GetByIdForPracticeAsync(Guid practiceId, Guid id);
    }
}
