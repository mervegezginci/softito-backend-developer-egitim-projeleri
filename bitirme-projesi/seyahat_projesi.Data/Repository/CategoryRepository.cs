using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;

namespace seyahat_projesi.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
