namespace SSIS.Models
{
    public class Employee : User
    {
        public string DepartmentCode { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("DepartmentCode")]
        public virtual Department Department { get; set; }
    }
}
