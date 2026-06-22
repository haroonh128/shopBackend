namespace Shop.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IOtpRepository OtpCodes { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IAuditLogRepository AuditLogs { get; }
        ILicenseRepository Licenses { get; }
        ILicenseSubscriptionRepository LicenseSubscriptions { get; }
        IPackageRepository Packages { get; }
        IModuleRepository Modules { get; }
        IPackageModuleRepository PackageModules { get; }
        IProductRepository Products { get; }
        IAddressRepository Addresses { get; }
        IStoreItemRepository StoreItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
