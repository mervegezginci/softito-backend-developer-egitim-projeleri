using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kuafor_ORMproje.Model
{
    public class Service
    {
        public int Id { get; set; }

        public string ServiceName { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
