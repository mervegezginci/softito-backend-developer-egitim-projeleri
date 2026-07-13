using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;

namespace seyahat_projesi.Data.Repository
{
    public class GuideRepository : Repository<Guide>, IGuideRepository
    {
        public GuideRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
