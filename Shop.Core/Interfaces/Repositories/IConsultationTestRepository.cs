using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IConsultationTestRepository : IRepository<ConsultationTest>
    {
        Task<IEnumerable<ConsultationTest>> GetByConsultationIdAsync(Guid consultationId);
    }
}
