using SSIS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSIS.View_Models
{
    public class GeneratePredictionViewModel
    {

        public List<Item> Items { get; set; }
        [Required]
        public DateTime ToDate { get; set; }
        [Required]
        public DateTime FromDate { get; set; }

        public string itemcode { get; set; }
        public Item Item { get; set; }



        public List<QuantityRecommendationViewModel> quantityRecommendationViewModels { get; set; }


    }
}
