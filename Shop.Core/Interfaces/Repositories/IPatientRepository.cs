using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetByIdForPracticeAsync(Guid practiceId, Guid id);
        Task<Patient?> GetByMRNumberAsync(Guid practiceId, string mrNumber);
        Task<string?> GetLatestMRNumberAsync(Guid practiceId);
        Task<(IEnumerable<Patient> Items, int TotalCount)> SearchAsync(
            Guid practiceId,
            string? mrNumber,
            string? name,
            string? contactNumber,
            string? gender,
            DateTime? dateOfBirth,
            DateTime? registrationFrom,
            DateTime? registrationTo,
            string? sortBy,
            bool sortDesc,
            int page,
            int pageSize);
    }
}
