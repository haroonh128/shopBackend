using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class ConsultationVitalsRepository : Repository<ConsultationVitals>, IConsultationVitalsRepository
    {
        public ConsultationVitalsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ConsultationVitals?> GetByConsultationIdAsync(Guid consultationId)
        {
            return await DbSet.FirstOrDefaultAsync(v => v.ConsultationId == consultationId && !v.IsDeleted);
        }
    }
}
