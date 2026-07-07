using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;

namespace kuafor_ORMproje.Data.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
