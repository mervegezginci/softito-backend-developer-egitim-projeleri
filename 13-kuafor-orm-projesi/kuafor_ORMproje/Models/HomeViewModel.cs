using kuafor_ORMproje.Model;

namespace kuafor_ORMproje.Models
{
    public class HomeViewModel
    {
        public List<Service> Services { get; set; }

        public List<Employee> Employees { get; set; }

        public List<Customer> Customers { get; set; }
    }
}
