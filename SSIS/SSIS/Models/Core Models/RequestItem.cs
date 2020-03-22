using System.ComponentModel.DataAnnotations;

namespace SSIS.Models
{
    public class RequestItem
    {
        [Key]
        public int RequestItemId { get; set; }
        public Item Item { get; set; }
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
