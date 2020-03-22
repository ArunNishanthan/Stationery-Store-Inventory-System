using SSIS.Enums;
using SSIS.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace SSIS.Services
{
    public class DepartmentServices
    {
        private SSISDbContext dbContext;

        public DepartmentServices(SSISDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Department UpdateDepartment(int userid, int collectionPointid)
        {
            Employee employee = dbContext.Employees.SingleOrDefault(m => m.UserId == userid);
            CollectionPoint cp = dbContext.CollectionPoints.SingleOrDefault(m => m.CollectionPointId == collectionPointid);

            var departmentDb = GetDepartment(employee.DepartmentCode);
            departmentDb.CollectionPoint = cp;
            departmentDb.DepartmentRepresentative = employee;

            dbContext.SaveChanges();

            return departmentDb;
        }
        public Department GetDepartment(String deptCode)
        {
            return dbContext.Departments.Include(m => m.DepartmentRepresentative).Include(m => m.DepartmentActingHead).Include(m => m.DepartmentHead).Include(m => m.CollectionPoint).SingleOrDefault(m => m.DepartmentCode == deptCode);
        }

        public void ChangeRoleForEmployee(int id)
        {
            var employee = dbContext.Employees.Include(m => m.Department.DepartmentRepresentative).SingleOrDefault(m => m.UserId == id);
            var department = GetDepartment(employee.DepartmentCode);
            Employee repEmployee = null;
            if (department.DepartmentRepresentative != null)
            {
                repEmployee = dbContext.Employees.SingleOrDefault(m => m.UserId == department.DepartmentRepresentative.UserId);
            }
            if (repEmployee != null && repEmployee.Role == Role.DEPT_REP)
            {
                repEmployee.Role = Role.EMPLOYEE;
            }
            employee.Role = Role.DEPT_REP;
            dbContext.SaveChanges();
        }

    }
}
