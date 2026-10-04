using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IPrescriptionItemRepository : IRepository<PrescriptionItem>
    {
        Task<IEnumerable<PrescriptionItem>> GetByPrescriptionIdAsync(Guid prescriptionId);
    }
}
