using SSIS.DTO;
using SSIS.Enums;
using SSIS.Models;
using SSIS.Services;
using SSIS.View_Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace SSIS.Controllers.api
{
    /*
     * Arun Nishanthan Anbalagan
     */
    public class EmployeeController : ApiController
    {
        private SSISDbContext dbContext = new SSISDbContext();

        private ItemServices itemServices;
        private EmployeeServices employeeServices;
        private CollectionPointServices collectionPointServices;
        private DepartmentServices departmentServices;
        private DelegationServices delegationServices;
        private EmailServices emailServices;
        private DashboardServices dashboardServices;
        public EmployeeController()
        {
            itemServices = new ItemServices(dbContext);
            employeeServices = new EmployeeServices(dbContext);
            collectionPointServices = new CollectionPointServices(dbContext);
            departmentServices = new DepartmentServices(dbContext);
            delegationServices = new DelegationServices(dbContext);
            emailServices = new EmailServices();
            dashboardServices = new DashboardServices(dbContext);
        }
        [HttpGet]
        public IHttpActionResult AssignDeptRepForm(int empId)
        {
            Employee employeeDep = employeeServices.GetEmployee(empId);
            var assignDeptRepViewModel = new AssignDeptRepViewModel
            {
                AssignTo = new Employee(),
                CollectionPoint = new CollectionPoint(),
                Department = employeeDep.Department,
                Employees = employeeServices.GetAllEmployeesByDepartment(employeeDep.DepartmentCode),
                CollectionPoints = collectionPointServices.GetCollectionPoint()
            };

            List<Employee> employees = new List<Employee>();
            foreach (Employee emp in assignDeptRepViewModel.Employees)
            {
                if (emp.Role.Equals(Role.DEPT_HEAD) || emp.Role.Equals(Role.DEPT_ACT_HEAD) || emp.Role == Role.DEPT_REP)
                {
                    continue;
                }
                else
                {
                    employees.Add(emp);
                }
            }

            assignDeptRepViewModel.Employees = employees;

            return Ok(assignDeptRepViewModel);
        }

        [HttpPost]
        public IHttpActionResult ChangeRep(ChangeRep changeRep)
        {
            var employee = employeeServices.GetEmployeeByName(changeRep.EmployeeName);
            departmentServices.ChangeRoleForEmployee(employee.UserId);
            var collectionPoint = collectionPointServices.GetCollectionPointbyName(changeRep.CollectionPointName);
            departmentServices.UpdateDepartment(employee.UserId, collectionPoint.CollectionPointId);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult DelegateAuthorityForm(int empId)
        {
            Employee employee = employeeServices.GetEmployee(empId);
            var delegateAuthorityViewModel = new DelegateAuthorityViewModel
            {
                DelegationDTO = new DelegationDTO()
            };
            List<Employee> employees = new List<Employee>();
            delegateAuthorityViewModel.Employees = employeeServices.GetAllEmployeesByDepartment(employee.DepartmentCode);
            foreach (Employee emp in delegateAuthorityViewModel.Employees)
            {
                if (!emp.Role.Equals(Role.DEPT_HEAD))
                {
                    employees.Add(emp);
                }
            }
            delegateAuthorityViewModel.Employees = employees;
            return Ok(delegateAuthorityViewModel);
        }

        [HttpPost]
        public IHttpActionResult SetDelegation(DelegationDTO delegationDto)
        {
            DateTime fromDate = DateTime.ParseExact(delegationDto.FromDate, "d/M/yyyy", null);
            DateTime toDate = DateTime.ParseExact(delegationDto.ToDate, "d/M/yyyy", null);


            Employee employee = employeeServices.GetEmployeeByName(delegationDto.DelegatedTo.UserName);
            Delegation delegation = new Delegation
            {
                DelegatedTo = employee,
                FromDate = fromDate,
                ToDate = toDate
            };
            delegationServices.SaveDelegation(delegation);
            if (fromDate.ToString("dd/MM/YYYY") == DateTime.Now.ToString("dd/MM/YYYY"))
            {
                employeeServices.ChangeRoleEmployee(employee.UserId, Role.DEPT_ACT_HEAD);

                //send Email to Employee
                String subject1 = "Delegate Authority";
                String body1 = "You're assigned as \"Acting Department Head\" from " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                emailServices.SentEmailTo(employee.Email, subject1, body1);

            }

            return Ok();
        }
        [HttpGet]
        public IHttpActionResult GetDelegationHistory(int empId)
        {
            Employee employeeDep = employeeServices.GetEmployee(empId);
            var delegateHistoryViewModel = new DelegationHistoryViewModel
            {
                Delegations = delegationServices.GetDelegationsbyDep(employeeDep.DepartmentCode)
            };
            return Ok(delegateHistoryViewModel);
        }

        [HttpGet]
        public IHttpActionResult CheckDelegation(string empname, string FromDate, string ToDate)
        {
            int status = 1;
            Employee employee = employeeServices.GetEmployeeByName(empname);
            var delegationList = delegationServices.GetDelegationsbyDep(employee.DepartmentCode);
            DateTime fromDate = DateTime.ParseExact(FromDate, "d/M/yyyy", null);
            DateTime toDate = DateTime.ParseExact(ToDate, "d/M/yyyy", null);

            foreach (var delegation in delegationList)
            {
                if ((fromDate >= delegation.FromDate && fromDate <= delegation.ToDate) || (delegation.FromDate >= fromDate && delegation.FromDate <= toDate))
                {
                    status = 0;
                }
            }
            return Ok(status);
        }


    }
}
