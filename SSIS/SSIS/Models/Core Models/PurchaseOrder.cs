using SSIS.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SSIS.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        public String PurchaseOrderNumber { get; set; }
        public Supplier Supplier { get; set; }
        [Column(TypeName = "datetime2")]

        public DateTime PurchaseOrderDate { get; set; }
        public PurchaseOrderStatus PurchaseOrderStatus { get; set; }
        [Column(TypeName = "datetime2")]

        public DateTime DeliveredDate { get; set; }
        public Collection<PurchaseItem> PurchaseItems { get; set; }
    }
}
