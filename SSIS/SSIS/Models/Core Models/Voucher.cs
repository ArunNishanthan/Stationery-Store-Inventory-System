using SSIS.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSIS.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }
        public Item Item { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Comment { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }
        public VoucherStatus VoucherStatus { get; set; }
    }
}
