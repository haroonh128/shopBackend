using Shop.Entities;

namespace Shop.Core.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByRegistrationNumberAsync(string registrationNumber);
    }
}