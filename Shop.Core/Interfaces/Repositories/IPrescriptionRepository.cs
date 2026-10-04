using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        Task<Prescription?> GetByConsultationIdAsync(Guid consultationId);
    }
}
