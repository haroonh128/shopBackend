using Shop.Entities;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Shop.Infrastructure.Repositories
{


    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalized = email.Trim().ToLower();
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized && !u.IsDeleted);
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber, Guid? excludeUserId = null)
        {
            var query = DbSet.Where(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(u => u.Active && !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllRegisteredUsersAsync(string? search = null)
        {
            var query = DbSet.AsNoTracking().Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(u =>
                    u.PhoneNumber.ToLower().Contains(term) ||
                    u.Email.ToLower().Contains(term) ||
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term) ||
                    u.CNIC.ToLower().Contains(term));
            }

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> AnyAdminExistsAsync()
        {
            return await DbSet.AnyAsync(u => u.IsAdmin && !u.IsDeleted);
        }

        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }
    }
}