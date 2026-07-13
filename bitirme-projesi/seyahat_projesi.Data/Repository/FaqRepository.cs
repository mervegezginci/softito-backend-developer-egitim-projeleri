using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;

namespace seyahat_projesi.Data.Repository
{
    public class FaqRepository : Repository<Faq>, IFaqRepository
    {
        public FaqRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
