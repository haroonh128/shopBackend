using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Prescription?> GetByConsultationIdAsync(Guid consultationId)
        {
            return await DbSet.FirstOrDefaultAsync(p => p.ConsultationId == consultationId && !p.IsDeleted);
        }
    }
}
