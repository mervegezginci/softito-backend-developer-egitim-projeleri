using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;

namespace kuafor_ORMproje.Data.Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
