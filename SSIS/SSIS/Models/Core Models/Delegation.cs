using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSIS.Models
{
    public class Delegation
    {
        [Key]
        public int Id { get; set; }
        public Employee DelegatedTo { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime? FromDate { get; set; }
        [Column(TypeName = "datetime2")]
        [Required]
        public DateTime? ToDate { get; set; }
    }
}
