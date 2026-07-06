using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace stok_codefirstmvcproje.model
{
    public class StockEntry
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        [Required]
        public int Quantity { get; set; }

        public DateTime EntryDate { get; set; }
    }
}