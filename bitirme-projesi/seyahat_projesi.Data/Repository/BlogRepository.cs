using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;

namespace seyahat_projesi.Data.Repository
{
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        public BlogRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
