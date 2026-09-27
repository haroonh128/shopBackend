using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class ConsultationDiagnosisRepository : Repository<ConsultationDiagnosis>, IConsultationDiagnosisRepository
    {
        public ConsultationDiagnosisRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ConsultationDiagnosis>> GetByConsultationIdAsync(Guid consultationId)
        {
            return await DbSet
                .Where(d => d.ConsultationId == consultationId && !d.IsDeleted)
                .ToListAsync();
        }
    }
}
