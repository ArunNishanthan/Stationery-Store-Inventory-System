using SSIS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSIS.View_Models
{
    public class Retrieval
    {
        public Department Department { get; set; }
        public int Needed { get; set; }
        [Range(0, 100)]
        [Required(ErrorMessage ="This value is required!")]
        public int Actual { get; set; }
    }
}