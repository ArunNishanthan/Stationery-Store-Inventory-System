using SSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.View_Models
{
    public class DelegationHistoryViewModel
    {
        public List<Delegation> Delegations { get; set; }
        public List<Employee> Employees { get; set; }
    }
}