using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<List<AuditLog>> GetUserActivityAsync(Guid userId, int count = 50);
        Task<List<AuditLog>> GetEntityHistoryAsync(string entityName, Guid entityId);
    }
}
