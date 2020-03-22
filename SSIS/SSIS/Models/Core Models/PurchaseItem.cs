using System.ComponentModel.DataAnnotations;

namespace SSIS.Models
{
    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
