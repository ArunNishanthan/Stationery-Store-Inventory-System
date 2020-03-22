using SSIS.Models;
using System;
using System.Collections.Generic;

namespace SSIS.View_Models
{
    public class ReorderReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int SelectedStatus { get; set; }
        public List<PurchaseOrder> PurchaseOrderLists { get; set; }
    }
}
