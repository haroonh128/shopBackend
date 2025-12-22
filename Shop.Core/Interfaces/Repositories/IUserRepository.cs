using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
        Task<IEnumerable<User>> GetActiveUsersAsync();
    }
}
