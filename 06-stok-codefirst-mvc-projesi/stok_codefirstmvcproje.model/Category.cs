using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stok_codefirstmvcproje.model
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        // Bir kategoride birden fazla ürün olabilir
        public List<Product> Products { get; set; }
    }
}
