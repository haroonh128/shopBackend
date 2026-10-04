using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class ConsultationRepository : Repository<Consultation>, IConsultationRepository
    {
        public ConsultationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Consultation?> GetByIdForPracticeAsync(Guid practiceId, Guid id)
        {
            return await DbSet
                .FirstOrDefaultAsync(c => c.Id == id && c.PracticeId == practiceId && !c.IsDeleted);
        }

        public async Task<IEnumerable<Consultation>> GetByPatientIdAsync(Guid practiceId, Guid patientId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(c => c.PracticeId == practiceId && c.PatientId == patientId && !c.IsDeleted)
                .OrderByDescending(c => c.VisitDate)
                .ToListAsync();
        }

        public async Task<string?> GetLatestVisitNumberAsync(Guid practiceId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(c => c.PracticeId == practiceId && !c.IsDeleted)
                .OrderByDescending(c => c.VisitNumber)
                .Select(c => c.VisitNumber)
                .FirstOrDefaultAsync();
        }
    }
}
