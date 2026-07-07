using System;

namespace kuafor_ORMproje.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customer { get; }
        IEmployeeRepository Employee { get; }
        IServiceRepository Service { get; }
        IAppointmentRepository Appointment { get; }
        IPaymentRepository Payment { get; }
        void Save();
    }
}
