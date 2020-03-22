using SSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.View_Models
{
    public class GenerateCategoryViewModel
    {
        

        public List<string> categorylist { get; set; }

        public string catCode { get; set; }

        public List<ReportItem> ReportItems { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime FromDate { get; set; }


    }
}