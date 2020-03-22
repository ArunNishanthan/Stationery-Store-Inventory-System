using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.Models
{
    public class ReportItem
    {
        public Item Item { get; set; }
        public int DisbursedQty { get; set; }
        public int RequestedQty { get; set; }
       
    }
}