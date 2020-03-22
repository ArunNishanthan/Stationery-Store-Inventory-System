using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.Models
{
    public class RecommendRes
    {
        [JsonProperty("recommend_res")]
        public List<RecommendResponse> recommend_res { get; set; }
    }
}
