using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IConsultationRepository : IRepository<Consultation>
    {
        Task<Consultation?> GetByIdForPracticeAsync(Guid practiceId, Guid id);
        Task<IEnumerable<Consultation>> GetByPatientIdAsync(Guid practiceId, Guid patientId);
        Task<string?> GetLatestVisitNumberAsync(Guid practiceId);
    }
}
