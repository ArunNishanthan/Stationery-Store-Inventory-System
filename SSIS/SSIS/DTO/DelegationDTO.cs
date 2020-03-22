using SSIS.Models;
using System.ComponentModel.DataAnnotations;

namespace SSIS.DTO
{
    public class DelegationDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Employee DelegatedTo { get; set; }
        [Required]
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Invalid date format.")]
        public string FromDate { get; set; }
        [RegularExpression(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$", ErrorMessage = "Invalid date format.")]
        [Required]
        public string ToDate { get; set; }
    }
}
