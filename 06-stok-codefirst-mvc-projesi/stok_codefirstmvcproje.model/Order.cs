using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stok_codefirstmvcproje.model
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public int Quantity { get; set; }

        // Hangi ürün sipariş edildi
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
