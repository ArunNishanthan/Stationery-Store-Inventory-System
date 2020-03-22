using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.Models
{
    public class RecommendResponse
    {
        [JsonProperty("item_number")]
        public string ItemNumber { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }
    }
}