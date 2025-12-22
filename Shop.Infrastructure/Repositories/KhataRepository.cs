using Shop.Entities;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class KhataRepository:Repository<Khata>, IKhataRepository    
    {
        public KhataRepository(ApplicationDbContext context) : base(context)
        {
        }


    }
}
