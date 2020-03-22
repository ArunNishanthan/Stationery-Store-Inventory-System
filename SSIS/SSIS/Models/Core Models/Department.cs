using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SSIS.Models
{
    public class Department
    {
        /*
         *  Department Code
            Department Name
            Department Head (Class)
            Department Representative (Class)
            Department Acting Head (Class)
            Default Collection Point (Class)
            Collection of Employee
            Collection of Disbursement List
         */
        [Key]
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public Employee DepartmentHead { get; set; }
        public Employee DepartmentRepresentative { get; set; }

        public Employee DepartmentActingHead { get; set; }

        public CollectionPoint CollectionPoint { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("DepartmentCode")]
        public Collection<Employee> Employees { get; set; }
        public Collection<DepDisbursementList> DepDisbursementLists { get; set; }

    }
}
