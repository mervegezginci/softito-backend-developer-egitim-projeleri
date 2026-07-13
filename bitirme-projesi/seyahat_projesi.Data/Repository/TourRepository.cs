using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;

namespace seyahat_projesi.Data.Repository
{
    public class TourRepository : Repository<Tour>, ITourRepository
    {
        public TourRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
