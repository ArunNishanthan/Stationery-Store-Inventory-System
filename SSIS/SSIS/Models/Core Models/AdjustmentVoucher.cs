using SSIS.Enum;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSIS.Models
{
    public class AdjustmentVoucher
    {

        [Key]
        public int VoucherId { get; set; }
        public String VoucherNumber { get; set; }
        public Item Item { get; set; }
        public int AdjustedQuantity { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime RequestedDate { get; set; }
        public StorePerson RequestedBy { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime IssuedDate { get; set; }
        public StorePerson IssuedBy { get; set; }
        public AdjustmentVoucherStatus VoucherStatus { get; set; }
        public Collection<Voucher> Vouchers { get; set; }
    }
}
