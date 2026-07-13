using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;

namespace seyahat_projesi.Data.Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
