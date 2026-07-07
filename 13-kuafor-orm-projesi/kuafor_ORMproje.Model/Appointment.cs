using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kuafor_ORMproje.Model
{
    public class Appointment
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public int ServiceId { get; set; }

        public Service? Service { get; set; }

        public DateTime AppointmentDate { get; set; }

        public TimeSpan AppointmentTime { get; set; }

        public string? Status { get; set; }

        public string? Note { get; set; }

        public Payment? Payment { get; set; }
    }
}
