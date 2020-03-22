using SSIS.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSIS.Models
{
    public class RequisationForm
    {

        [Key]
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public Employee RequestedBy { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? RequestedDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? HandeledDate { get; set; }
        public Employee HandeledBy { get; set; }
        public String Comments { get; set; }

        public FormStatus FormStatus { get; set; }
        public Collection<RequestItem> RequestItems { get; set; }


    }
}
