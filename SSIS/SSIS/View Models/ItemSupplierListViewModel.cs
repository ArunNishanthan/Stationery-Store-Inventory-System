using SSIS.Models;
using System;
using System.Collections.Generic;

namespace SSIS.View_Models
{
    public class ItemSupplierListViewModel
    {
        public List<Supplier> Suppliers { get; set; }
        public String SelectedSupplierCode { get; set; }
        public List<ItemSupplier> itemSuppliers { get; set; }
    }
}
