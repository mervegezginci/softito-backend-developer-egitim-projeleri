using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;

namespace kuafor_ORMproje.Data.Repository
{
    public class ServiceRepository : Repository<Service>, IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
