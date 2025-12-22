
namespace Shop.Infrastructure.Services
{
    public interface IAuditService
    {
        Task LogActionAsync(Guid? userId, string action, string entityName, string? entityId = null, string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null);
    }
}