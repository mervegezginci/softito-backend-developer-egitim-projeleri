using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stok_codefirstmvcproje.model
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        // İlişki: Bir tedarikçiden birden fazla ürün gelebilir
        public List<Product> Products { get; set; } = new();
    }
}
