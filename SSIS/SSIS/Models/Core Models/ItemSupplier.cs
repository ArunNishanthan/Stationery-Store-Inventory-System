using System;
using System.ComponentModel.DataAnnotations;

namespace SSIS.Models
{
    public class ItemSupplier
    {
        [Key]
        public int ItemSupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public Item Item { get; set; }
        public Double Price { get; set; }
        public int Priority { get; set; }
    }
}
