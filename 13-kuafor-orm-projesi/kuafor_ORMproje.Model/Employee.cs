using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kuafor_ORMproje.Model
{
    public class Employee
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Speciality { get; set; }

        public string ImageUrl { get; set; }

        public string Phone { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
