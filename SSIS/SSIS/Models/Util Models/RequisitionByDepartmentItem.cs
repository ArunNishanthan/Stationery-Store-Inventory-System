using SSIS.Models;
using System;
using System.Collections.Generic;

namespace SSIS.View_Models
{
    public class RequisitionByDepartmentItem
    {
        public Department Department { get; set; }
        public DateTime? LastUpdated { get; set; }
        public List<RequestItem> RequestItems { get; set; }
    }
}
