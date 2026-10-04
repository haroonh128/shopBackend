using Microsoft.EntityFrameworkCore;
using Shop.Common.enums;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByIdForPracticeAsync(Guid practiceId, Guid id)
        {
            return await DbSet
                .FirstOrDefaultAsync(p => p.Id == id && p.PracticeId == practiceId && !p.IsDeleted);
        }

        public async Task<Patient?> GetByMRNumberAsync(Guid practiceId, string mrNumber)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PracticeId == practiceId && p.MRNumber == mrNumber && !p.IsDeleted);
        }

        public async Task<string?> GetLatestMRNumberAsync(Guid practiceId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(p => p.PracticeId == practiceId && !p.IsDeleted)
                .OrderByDescending(p => p.MRNumber)
                .Select(p => p.MRNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Patient> Items, int TotalCount)> SearchAsync(
            Guid practiceId,
            string? mrNumber,
            string? name,
            string? contactNumber,
            string? gender,
            DateTime? dateOfBirth,
            DateTime? registrationFrom,
            DateTime? registrationTo,
            string? sortBy,
            bool sortDesc,
            int page,
            int pageSize)
        {
            var query = DbSet.AsNoTracking().Where(p => p.PracticeId == practiceId && !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(mrNumber))
                query = query.Where(p => p.MRNumber.Contains(mrNumber.Trim()));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.FullName.Contains(name.Trim()));

            if (!string.IsNullOrWhiteSpace(contactNumber))
                query = query.Where(p => p.ContactNumber.Contains(contactNumber.Trim())
                    || (p.AlternateContactNumber != null && p.AlternateContactNumber.Contains(contactNumber.Trim())));

            if (!string.IsNullOrWhiteSpace(gender) && Enum.TryParse<Gender>(gender, true, out var genderValue))
                query = query.Where(p => p.Gender == genderValue);

            if (dateOfBirth.HasValue)
                query = query.Where(p => p.DateOfBirth.HasValue && p.DateOfBirth.Value.Date == dateOfBirth.Value.Date);

            if (registrationFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= registrationFrom.Value.Date);

            if (registrationTo.HasValue)
            {
                var to = registrationTo.Value.Date.AddDays(1);
                query = query.Where(p => p.CreatedAt < to);
            }

            query = (sortBy?.ToLowerInvariant()) switch
            {
                "mrnumber" => sortDesc ? query.OrderByDescending(p => p.MRNumber) : query.OrderBy(p => p.MRNumber),
                "contactnumber" => sortDesc ? query.OrderByDescending(p => p.ContactNumber) : query.OrderBy(p => p.ContactNumber),
                "createdat" => sortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => sortDesc ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName)
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip(Math.Max(0, (page - 1) * pageSize))
                .Take(pageSize <= 0 ? 20 : pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
