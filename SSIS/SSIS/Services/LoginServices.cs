using SSIS.Enums;
using SSIS.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace SSIS.Services
{
    public class LoginServices
    {
        private SSISDbContext dbContext = new SSISDbContext();

        public Employee CheckEmployee(string Email)
        {

            return dbContext.Employees.SingleOrDefault(e => e.Email == Email);
        }



        public StorePerson CheckStorePerson(string Email)
        {
            return dbContext.StorePersons.SingleOrDefault(e => e.Email == Email);

        }

        public void CheckDelegationTable(User user)
        {
            var delegationList = dbContext.Delegations.Include(m => m.DelegatedTo)
                .Where(m => m.DelegatedTo.UserId == user.UserId).Where(m => m.FromDate <= DateTime.Today && m.ToDate >= DateTime.Today).ToList();

            Employee employee = dbContext.Employees.SingleOrDefault(e => e.UserId == user.UserId);
            if (employee.Role == Role.DEPT_REP)
            {
                employee.Role = Role.DEPT_REP;
            }
            else if (employee.Role == Role.DEPT_HEAD)
            {
                employee.Role = Role.DEPT_HEAD;
            }
            else
            {
                employee.Role = Role.EMPLOYEE;
            }
            if (delegationList.Count > 0)
                employee.Role = Role.DEPT_ACT_HEAD;
            dbContext.SaveChanges();
        }

    }
}
