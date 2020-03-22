
using SSIS.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSIS.Models
{
    public class DepDisbursementList
    {
        [Key]
        public int DepDisbursementListId { get; set; }
        public string DepDisbursementListNumber { get; set; }
        public Department Department { get; set; }
        public string OTP { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime DisburseDate { get; set; }
        public DisbursementStatus DisbursementStatus { get; set; }
        public Collection<DisburseItem> DisburseItems { get; set; }
        public Boolean isGenerated { get; set; }
    }
}
