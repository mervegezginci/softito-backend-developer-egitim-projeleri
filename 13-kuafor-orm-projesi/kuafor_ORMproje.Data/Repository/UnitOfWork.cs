using kuafor_ORMproje.Data.Repository.IRepository;
using System;

namespace kuafor_ORMproje.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICustomerRepository Customer => new CustomerRepository(_context);
        public IEmployeeRepository Employee => new EmployeeRepository(_context);
        public IServiceRepository Service => new ServiceRepository(_context);
        public IAppointmentRepository Appointment => new AppointmentRepository(_context);
        public IPaymentRepository Payment => new PaymentRepository(_context);

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
