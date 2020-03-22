using System.ComponentModel.DataAnnotations;

namespace SSIS.Models
{
    public class DisburseItem
    {

        [Key]
        public int DisburseItemID { get; set; }
        public Item RequestItem { get; set; }
        public int RequestedQuantity { get; set; }
        public int RetrivedQuantity { get; set; }
        public int DisbursedQuantity { get; set; }
        public DepDisbursementList DepDisbursementList { get; set; }
    }
}
