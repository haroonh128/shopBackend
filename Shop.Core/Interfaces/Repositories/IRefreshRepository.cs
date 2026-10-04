using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetValidTokenAsync(string token);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task<List<RefreshToken>> GetActiveUserTokensAsync(Guid userId);
        Task<int> GetActiveTokenCountAsync(Guid userId);
    }
}
