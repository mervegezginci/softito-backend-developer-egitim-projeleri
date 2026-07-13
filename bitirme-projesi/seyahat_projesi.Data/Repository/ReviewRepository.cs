using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;

namespace seyahat_projesi.Data.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
