using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class ConsultationTestRepository : Repository<ConsultationTest>, IConsultationTestRepository
    {
        public ConsultationTestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ConsultationTest>> GetByConsultationIdAsync(Guid consultationId)
        {
            return await DbSet
                .Where(t => t.ConsultationId == consultationId && !t.IsDeleted)
                .ToListAsync();
        }
    }
}
