using SSIS.Models;
using System;

namespace SSIS.View_Models
{
    public class RequisitionByDepartment
    {
        public Department Department { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
