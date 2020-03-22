using SSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.View_Models
{
    public class GenerateReportViewModel
    {
        public List<Department> Departments { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime FromDate { get; set; }

        public List<ReportItem> ReportItems { get; set; }
        public string DepCode { get; set; }

    }
}