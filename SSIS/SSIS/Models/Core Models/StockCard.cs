using System;

namespace SSIS.Models
{
    public class StockCard
    {
        public int StockCardId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int ChangeInQuantity { get; set; }
        public int Balance { get; set; }
        public string Comments { get; set; }
        public Item Item { get; set; }
    }
}
