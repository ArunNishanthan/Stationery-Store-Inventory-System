using SSIS.DTO;
using SSIS.Enums;
using SSIS.Models;
using SSIS.Security.Filter;
using SSIS.Services;
using SSIS.View_Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SSIS.Controllers
{
    /*
     *Coded by
     * Arun Nishanthan Anbalagan
     * Raja Sudalaimuthu Padma
     */
    [SessionCheck]
    [Authorize(Roles = "DEPT_HEAD")]
    public class DeptHeadController : Controller
    {
        private SSISDbContext dbContext = new SSISDbContext();

        private EmployeeServices employeeServices;
        private DelegationServices delegationServices;
        private DepartmentServices departmentServices;
        private CollectionPointServices collectionPointServices;
        private EmailServices emailServices;
        private StorePersonServices storepersonServices;

        public DeptHeadController()
        {
            employeeServices = new EmployeeServices(dbContext);
            delegationServices = new DelegationServices(dbContext);
            departmentServices = new DepartmentServices(dbContext);
            collectionPointServices = new CollectionPointServices(dbContext);
            emailServices = new EmailServices();
            storepersonServices = new StorePersonServices(dbContext);
        }

        [HttpGet]
        public ActionResult DelegateAuthorityForm()
        {
            Employee employee = (Employee)Session["usersession"];
            employee = employeeServices.GetEmployee(employee.UserId);
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
            return View(delegateAuthorityViewModel);
        }

        public ActionResult CancelDelegation(int id)
        {
            Employee employee = delegationServices.GetDelegatedTo(id);
            delegationServices.CancelDelegation(id);

            employeeServices.ChangeRoleEmployee(employee.UserId, Role.EMPLOYEE);
            return RedirectToAction("GetDelegationHistory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VaildDelegateAuthority(DelegateAuthorityViewModel delegateAuthorityViewModel)
        {
            var fromDate = DateTime.ParseExact(delegateAuthorityViewModel.DelegationDTO.FromDate, "dd/MM/yyyy", null);
            var toDate = DateTime.ParseExact(delegateAuthorityViewModel.DelegationDTO.ToDate, "dd/MM/yyyy", null);

            Employee employee = (Employee)Session["usersession"];
            employee = employeeServices.GetEmployee(employee.UserId);

            var delegationList = delegationServices.GetDelegationsbyDep(employee.DepartmentCode);

            foreach (var delegation in delegationList)
            {
                if ((fromDate >= delegation.FromDate && fromDate <= delegation.ToDate) || (delegation.FromDate >= fromDate && delegation.FromDate <= toDate))
                {
                    ModelState.AddModelError("DelegationDTO.DelegatedTo", "Employee already assigned");

                }
            }
            if (fromDate > toDate)
            {
                ModelState.AddModelError("DelegationDTO.FromDate", "Start Date cannot be greater than End Date");
            }

            if (ModelState.IsValid)
            {
                var delEmp = employeeServices.GetEmployee(delegateAuthorityViewModel.DelegationDTO.DelegatedTo.UserId);
                Delegation delegation = new Delegation
                {
                    DelegatedTo = delEmp,
                    FromDate = DateTime.ParseExact(delegateAuthorityViewModel.DelegationDTO.FromDate, "dd/MM/yyyy", null),
                    ToDate = DateTime.ParseExact(delegateAuthorityViewModel.DelegationDTO.ToDate, "dd/MM/yyyy", null)
                };
                delegationServices.SaveDelegation(delegation);
                String fDate = fromDate.ToString("dd/MM/YYYY");
                if (fDate == DateTime.Now.ToString("dd/MM/YYYY"))
                {
                    employeeServices.ChangeRoleEmployee(delEmp.UserId, Role.DEPT_ACT_HEAD);

                    //send Email to Employee
                    String subject1 = "Delegate Authority";
                    String body1 = "You're assigned as \"Acting Department Head\" from " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString();
                    emailServices.SentEmailTo(delEmp.Email, subject1, body1);
                }

                return RedirectToAction("GetDelegationHistory");
            }
            else
            {
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
            }
            return View("DelegateAuthorityForm", delegateAuthorityViewModel);
        }

        public ActionResult GetDelegationHistory()
        {
            Employee employeeDep = (Employee)Session["usersession"];
            employeeDep = employeeServices.GetEmployee(employeeDep.UserId);
            var delegateHistoryViewModel = new DelegationHistoryViewModel
            {
                Delegations = delegationServices.GetDelegationsbyDep(employeeDep.DepartmentCode)
            };
            return View("DelegationHistory", delegateHistoryViewModel);
        }

        public ActionResult AssignDeptRepForm()
        {
            Employee employeeDep = (Employee)Session["usersession"];
            employeeDep = employeeServices.GetEmployee(employeeDep.UserId);
            var assignDeptRepViewModel = new AssignDeptRepViewModel
            {
                AssignTo = new Employee(),
                CollectionPoint = new CollectionPoint(),
                Department = employeeDep.Department,
                Employees = employeeServices.GetAllEmployeesByDepartment(employeeDep.DepartmentCode),
                CollectionPoints = collectionPointServices.GetCollectionPoint()
            };
            Department department = departmentServices.GetDepartment(employeeDep.DepartmentCode);
            if (department.DepartmentRepresentative != null)
            {
                assignDeptRepViewModel.AssignTo = department.DepartmentRepresentative;
            }
            if (department.CollectionPoint != null)
            {
                assignDeptRepViewModel.CollectionPoint = department.CollectionPoint;
            }
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
            List<CollectionPoint> collectionPoints = new List<CollectionPoint>();

            foreach (var collectionPoint in assignDeptRepViewModel.CollectionPoints)
            {
                if (assignDeptRepViewModel.CollectionPoint.CollectionPointId != collectionPoint.CollectionPointId)
                {
                    collectionPoints.Add(collectionPoint);
                }
            }

            assignDeptRepViewModel.CollectionPoints = collectionPoints;
            assignDeptRepViewModel.Employees = employees;

            return View(assignDeptRepViewModel);
        }

        [HttpPost]
        public ActionResult ValidAssignDeptRep(AssignDeptRepViewModel assignDeptRepViewModel)
        {
            bool isvalid = true;

            Employee employeeDep = (Employee)Session["usersession"];
            employeeDep = employeeServices.GetEmployee(employeeDep.UserId);
            Department dep = departmentServices.GetDepartment(employeeDep.DepartmentCode);
            if (dep.DepartmentRepresentative == null && assignDeptRepViewModel.AssignTo.UserId == 0)
            {
                isvalid = false;
                ModelState.Clear();
                ModelState.AddModelError("AssignTo.UserId", "Select the User");
            }
            if (dep.CollectionPoint == null && assignDeptRepViewModel.CollectionPoint.CollectionPointId == 0)
            {
                isvalid = false;
                ModelState.Clear();
                ModelState.AddModelError("CollectionPoint.CollectionPointId", "Select the Collection Point");
            }

            if (assignDeptRepViewModel.CollectionPoint.CollectionPointId == 0 && assignDeptRepViewModel.AssignTo.UserId == 0)
            {
                isvalid = false;
                ModelState.Clear();
                ModelState.AddModelError("AssignTo.UserId", "Select the User");
                ModelState.AddModelError("CollectionPoint.CollectionPointId", "Select the Collection Point");
            }
            if (isvalid)
            {
                Employee emp = dep.DepartmentRepresentative;
                if (assignDeptRepViewModel.AssignTo.UserId != 0)
                {
                    emp = employeeServices.GetEmployee(assignDeptRepViewModel.AssignTo.UserId);
                }
                CollectionPoint cpt = dep.CollectionPoint;
                if (assignDeptRepViewModel.CollectionPoint.CollectionPointId != 0)
                {
                    cpt = collectionPointServices.GetOneCollectionPoint(assignDeptRepViewModel.CollectionPoint.CollectionPointId);
                }
                //send Email to Dep Rep
                String subject1 = "Assign: Representative";
                String body1 = "You're assigned as Department Representative.\nCollection Point is " + cpt.Place + ".";
                emailServices.SentEmailTo(emp.Email, subject1, body1);
                //send Email to  All StoreClerks
                List<StorePerson> storePersonList = storepersonServices.GetStorePerson();
                foreach (StorePerson s in storePersonList)
                {
                    String subject2 = "Department Representative and Collection Point: Change";
                    String body2 = "Department Name: " + dep.DepartmentName + "\nNew Representative: " + emp.UserName + "\nNew Collection Point: " + cpt.Place;
                    emailServices.SentEmailTo(s.Email, subject2, body2);
                }
                //end
                departmentServices.ChangeRoleForEmployee(emp.UserId);
                departmentServices.UpdateDepartment(emp.UserId, cpt.CollectionPointId);
            }

            dep = departmentServices.GetDepartment(employeeDep.DepartmentCode);

            assignDeptRepViewModel.AssignTo = dep.DepartmentRepresentative;
            assignDeptRepViewModel.CollectionPoint = collectionPointServices.GetOneCollectionPoint(dep.CollectionPoint.CollectionPointId);

            assignDeptRepViewModel.Department = dep;
            assignDeptRepViewModel.Employees = employeeServices.GetAllEmployeesByDepartment(dep.DepartmentCode);

            List<Employee> employees = new List<Employee>();
            foreach (Employee employee in assignDeptRepViewModel.Employees)
            {
                if (employee.Role.Equals(Role.DEPT_HEAD) || employee.Role.Equals(Role.DEPT_ACT_HEAD) || employee.Role == Role.DEPT_REP)
                {
                    continue;
                }
                else
                {
                    employees.Add(employee);
                }
            }

            assignDeptRepViewModel.Employees = employees;
            assignDeptRepViewModel.CollectionPoints = collectionPointServices.GetCollectionPoint();
            if (isvalid)
            {
                ModelState.Clear();
            }

            List<CollectionPoint> collectionPoints = new List<CollectionPoint>();

            foreach (var collectionPoint in assignDeptRepViewModel.CollectionPoints)
            {
                if (assignDeptRepViewModel.CollectionPoint.CollectionPointId != collectionPoint.CollectionPointId)
                {
                    collectionPoints.Add(collectionPoint);
                }
            }
            assignDeptRepViewModel.CollectionPoints = collectionPoints;
            Console.WriteLine(ModelState.Count);
            return View("AssignDeptRepForm", assignDeptRepViewModel);
        }
    }
}
