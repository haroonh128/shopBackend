using Microsoft.EntityFrameworkCore;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Doctor>> GetByPracticeIdAsync(Guid practiceId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(d => d.PracticeId == practiceId && !d.IsDeleted)
                .OrderBy(d => d.FullName)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdForPracticeAsync(Guid practiceId, Guid id)
        {
            return await DbSet
                .FirstOrDefaultAsync(d => d.Id == id && d.PracticeId == practiceId && !d.IsDeleted);
        }
    }
}
