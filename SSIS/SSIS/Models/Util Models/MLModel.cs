using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.Models
{
    public class MLModel
    {
        public string item_number { get; set; }
        public int year { get; set; }
        public int month { get; set; }

    }

    public class RecommendList
    {
        public List<MLModel> recommend_req { get; set; }
    }

}