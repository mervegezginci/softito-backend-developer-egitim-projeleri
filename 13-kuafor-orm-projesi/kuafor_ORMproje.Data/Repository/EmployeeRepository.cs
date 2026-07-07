using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;

namespace kuafor_ORMproje.Data.Repository
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
