using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IOtpRepository : IRepository<OtpCode>
    {
        Task<OtpCode?> GetValidOtpAsync(Guid userId, string code);
        Task<List<OtpCode>> GetExpiredOtpsAsync(DateTime cutoffTime);
        Task<int> GetActiveOtpCountAsync(Guid userId);
        Task<List<OtpCode>> GetUserOtpHistoryAsync(Guid userId, int count = 10);
    }
}
