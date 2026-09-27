using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IConsultationDiagnosisRepository : IRepository<ConsultationDiagnosis>
    {
        Task<IEnumerable<ConsultationDiagnosis>> GetByConsultationIdAsync(Guid consultationId);
    }
}
