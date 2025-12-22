namespace Shop.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IOtpRepository OtpCodes { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IAuditLogRepository AuditLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
