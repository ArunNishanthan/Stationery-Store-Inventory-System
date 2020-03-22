using SSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.View_Models
{
    public class QuantityRecommendationViewModel
    {
        public Item Item { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int recommend_Qty { get; set; }
        public string strMonth { get; set; }
    }
}