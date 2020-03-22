using SSIS.Models;
using System.Collections.Generic;

namespace SSIS.View_Models
{
    public class AssignDeptRepViewModel
    {
        public IEnumerable<CollectionPoint> CollectionPoints { get; set; }
        public List<Employee> Employees { get; set; }
        public Department Department { get; set; }
        public Employee AssignTo { get; set; }
        public CollectionPoint CollectionPoint { get; set; }

    }
}
