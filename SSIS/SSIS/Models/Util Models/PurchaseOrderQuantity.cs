using SSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.View_Models
{
    public class PurchaseOrderQuantity
    {
        public ItemSupplier itemSupplier { get; set; }
        public int Quantity { get; set; }
    }
}