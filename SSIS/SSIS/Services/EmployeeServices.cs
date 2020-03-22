using SSIS.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSIS.Services
{
    public class EmployeeServices
    {
        private SSISDbContext dbContext;

        public EmployeeServices(SSISDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public Employee GetEmployee(int userId)
        {
            return dbContext.Employees.Include(m => m.Department).SingleOrDefault(emp => emp.UserId == userId);
        }

        public List<Employee> GetAllEmployeesByDepartment(string depCode)
        {
            return dbContext.Employees.Where(emp => emp.DepartmentCode == depCode).ToList();
        }
        public Employee ChangeRoleEmployee(int userId, Enums.Role role)
        {
            var employeeDb = GetEmployee(userId);
            employeeDb.Role = role;
            dbContext.SaveChanges();
            return employeeDb;
        }
        public Employee GetDeptRepbyDepCode(string departmentCode)
        {
            return dbContext.Employees.Where(i => i.Department.DepartmentCode == departmentCode && i.Role == Enums.Role.DEPT_REP).SingleOrDefault();
        }
        public Employee GetDepartmentHead(string departmentCode)
        {
            return dbContext.Employees
                .Where(e => e.DepartmentCode == departmentCode && e.Role == Enums.Role.DEPT_HEAD)
                .SingleOrDefault();
        }

        public Employee GetEmployeeByReqForm(RequisationForm reqForm)
        {
            return dbContext.RequisationForms.Where(i => i.Id == reqForm.Id).Select(i => i.RequestedBy).FirstOrDefault();
        }

        public Employee GetEmployeeByName(string name)
        {
            return dbContext.Employees.Include(m => m.Department).Where(i => i.UserName == name).FirstOrDefault();
        }
    }
}
