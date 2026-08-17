using Microsoft.EntityFrameworkCore.Storage;
using Shop.Core.Interfaces.Repositories;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;

namespace Shop.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IUserRepository? _userRepository;
        private IOtpRepository? _otpRepository;
        private IRefreshTokenRepository? _refreshTokenRepository;
        private IAuditLogRepository? _auditLogRepository;
        private ILicenseRepository? _licenseRepository;
        private ILicenseSubscriptionRepository? _licenseSubscriptionRepository;
        private IPackageRepository? _packageRepository;
        private IModuleRepository? _moduleRepository;
        private IPackageModuleRepository? _packageModuleRepository;
        private IProductRepository? _productRepository;
        private IAddressRepository? _addressRepository;
        private IStoreItemRepository? _storeItemRepository;
        private IMeasurementRepository? _measurementRepository;
        private IOrderRepository? _orderRepository;
        private IClientRepository? _clientRepository;
        private ICostRepository? _costRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users =>
            _userRepository ??= new UserRepository(_context);

        public IOtpRepository OtpCodes =>
            _otpRepository ??= new OtpRepository(_context);

        public IRefreshTokenRepository RefreshTokens =>
            _refreshTokenRepository ??= new RefreshTokenRepository(_context);

        public IAuditLogRepository AuditLogs =>
            _auditLogRepository ??= new AuditLogRepository(_context);

        public ILicenseRepository Licenses =>
            _licenseRepository ??= new LicenseRepository(_context);

        public ILicenseSubscriptionRepository LicenseSubscriptions =>
            _licenseSubscriptionRepository ??= new LicenseSubscriptionRepository(_context);

        public IPackageRepository Packages =>
            _packageRepository ??= new PackageRepository(_context);

        public IModuleRepository Modules =>
            _moduleRepository ??= new ModuleRepository(_context);

        public IPackageModuleRepository PackageModules =>
            _packageModuleRepository ??= new PackageModuleRepository(_context);

        public IProductRepository Products =>
            _productRepository ??= new ProductRepository(_context);

        public IAddressRepository Addresses =>
            _addressRepository ??= new AddressRepository(_context);

        public IStoreItemRepository StoreItems => _storeItemRepository ??= new StoreItemRepository(_context);

        public IMeasurementRepository Measurements =>
            _measurementRepository ??= new MeasurementRepository(_context);

        public IOrderRepository Orders =>
            _orderRepository ??= new OrderRepository(_context);

        public IClientRepository Clients =>
            _clientRepository ??= new ClientRepository(_context);

        public ICostRepository Costs =>
            _costRepository ??= new CostRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
