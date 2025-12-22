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
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await DbSet
                .AnyAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(u => u.Active && !u.IsDeleted)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }
    }
}