using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;

namespace seyahat_projesi.Data.Repository
{
    public class ContactMessageRepository : Repository<ContactMessage>, IContactMessageRepository
    {
        public ContactMessageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
