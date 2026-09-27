using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IConsultationVitalsRepository : IRepository<ConsultationVitals>
    {
        Task<ConsultationVitals?> GetByConsultationIdAsync(Guid consultationId);
    }
}
