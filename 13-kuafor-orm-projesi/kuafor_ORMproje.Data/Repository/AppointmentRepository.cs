using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;

namespace kuafor_ORMproje.Data.Repository
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
