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
