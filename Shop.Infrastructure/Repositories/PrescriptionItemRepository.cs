using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class PrescriptionItemRepository : Repository<PrescriptionItem>, IPrescriptionItemRepository
    {
        public PrescriptionItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PrescriptionItem>> GetByPrescriptionIdAsync(Guid prescriptionId)
        {
            return await DbSet
                .Where(i => i.PrescriptionId == prescriptionId && !i.IsDeleted)
                .OrderBy(i => i.SortOrder)
                .ToListAsync();
        }
    }
}
