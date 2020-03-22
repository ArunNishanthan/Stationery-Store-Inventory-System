using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SSIS.Models
{
    public class Item
    {
        [Key]
        public String ItemCode { get; set; }
        public String Category { get; set; }
        public string Description { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public int CurrentQuantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public bool IsActive { get; set; }
        public Collection<ItemSupplier> ItemSuppliers { get; set; }

        public Collection<StockCard> StockCards { get; set; }
    }
}
