using SSIS.Enums;
using SSIS.Models;
using SSIS.Security.Filter;
using SSIS.Services;
using SSIS.View_Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;


namespace SSIS.Controllers
{
    /*
     *Coded by
     * Arun Nishanthan Anbalagan
     * Hay Mun Linn
     * Raja Sudalaimuthu Padma
     * Tang Zi Jian
     */
    [SessionCheck]
    [Authorize]
    public class EmployeeController : Controller
    {
        private SSISDbContext dbContext = new SSISDbContext();

        private ItemServices itemServices;
        private EmployeeServices employeeServices;
        private EmailServices emailServices;
        private StorePersonServices storepersonServices;
        private DashboardServices dashboardServices;
        public EmployeeController()
        {
            itemServices = new ItemServices(dbContext);
            employeeServices = new EmployeeServices(dbContext);
            emailServices = new EmailServices();
            storepersonServices = new StorePersonServices(dbContext);
            dashboardServices = new DashboardServices(dbContext);
        }

        [HttpGet]
        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,EMPLOYEE,DEPT_ACT_HEAD")]
        public ActionResult RequestForm()
        {
            var requisitionViewModel = new RequisitionViewModel
            {
                RequestItems = new Collection<RequestItem>()
            };
            return View(requisitionViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,EMPLOYEE,DEPT_ACT_HEAD")]
        public ActionResult CheckReqList(RequisitionViewModel requisitionViewModel)
        {
            bool flag = false;
            if (ModelState.IsValid)
            {
                var reqList = new Collection<RequestItem>();
                if (requisitionViewModel.RequestItems != null)
                {
                    reqList = requisitionViewModel.RequestItems;
                }
                requisitionViewModel.RequestItems = reqList;
                bool isAvailable = false;
                if (requisitionViewModel.RequestCode != null)
                {
                    var item = itemServices.GetItem(requisitionViewModel.RequestCode);
                    RequestItem requestItem = new RequestItem();
                    foreach (var rItem in requisitionViewModel.RequestItems)
                    {
                        if (rItem.Item.ItemCode == item.ItemCode)
                        {
                            isAvailable = true;
                        }

                        rItem.Item = itemServices.GetItem(rItem.Item.ItemCode);
                    }
                    if (!isAvailable)
                    {
                        requestItem.Item = item;
                        requestItem.Quantity = 1;
                        reqList.Add(requestItem);
                    }

                    requisitionViewModel.RequestCode = "";
                    requisitionViewModel.DeleteCode = "";
                    requisitionViewModel.RequestItems = reqList;
                }
                if (requisitionViewModel.DeleteCode != null)
                {
                    var item = reqList.FirstOrDefault(t => t.Item.ItemCode == requisitionViewModel.DeleteCode);
                    reqList.Remove(item);

                    foreach (var rItem in reqList)
                    {
                        rItem.Item = itemServices.GetItem(rItem.Item.ItemCode);
                    }

                    requisitionViewModel.RequestCode = "";
                    requisitionViewModel.DeleteCode = "";
                    requisitionViewModel.RequestItems = reqList;
                }
                else if (requisitionViewModel.DeleteCode == null && requisitionViewModel.RequestCode == null)
                {
                    var sessionUser = (User)Session["usersession"];
                    var requisationForm = new RequisationForm();
                    if (requisitionViewModel.RequisationFormID == 0)
                    {
                        requisationForm.RequestedBy = employeeServices.GetEmployee(sessionUser.UserId);
                    }
                    else
                    {
                        requisationForm = itemServices.GetRequisationFormbyId(requisitionViewModel.RequisationFormID);
                    }

                    foreach (var rt in reqList)
                    {
                        rt.Item = itemServices.GetItem(rt.Item.ItemCode);
                        itemServices.SaveRequestItem(rt);
                    }
                    requisationForm.RequestItems = reqList;
                    requisationForm.HandeledDate = DateTime.MinValue;
                    requisationForm.FormStatus = FormStatus.PENDING_APPROVAL;
                    if (requisationForm.RequestedBy.Role == Role.DEPT_HEAD || requisationForm.RequestedBy.Role == Role.DEPT_ACT_HEAD)
                    {
                        requisationForm.FormStatus = FormStatus.PENDING_ADDITION;
                        requisationForm.HandeledBy = requisationForm.RequestedBy;
                        requisationForm.HandeledDate = DateTime.Now;


                        //check whether request is done by head or not
                        flag = true;
                    }

                    requisationForm.RequestedDate = DateTime.Now;

                    if (requisationForm.RequestItems.Count != 0)
                    {
                        itemServices.SaveRequisationForm(requisationForm);

                        //if requested by head, then no need to sent mail
                        if (!flag)
                        {
                            //send Email to Employee
                            var user = (User)Session["usersession"];
                            Employee emp = employeeServices.GetEmployee(user.UserId);
                            RequisationForm form = itemServices.GetRequisationFormbyId(requisationForm.Id);
                            String subject1 = "Requisition Form Submitted:" + form.RequestNumber;
                            String body1 = "Requisition form: <a href='https://127.0.0.1:44366/Account/Login' >" +
                                           form.RequestNumber + " </a> is successfully submitted.";
                            emailServices.SentEmailTo(emp.Email, subject1, body1);
                            //send Email to DepHead
                            Employee deptHead = employeeServices.GetDepartmentHead(emp.Department.DepartmentCode);
                            String subject2 = "Requisition form: " + form.RequestNumber + ": Pending Approval";
                            String body2 =
                                "There is a new requisition form: <a href='https://127.0.0.1:44366/Account/Login' >" +
                                form.RequestNumber + " </a> to approve.";
                            emailServices.SentEmailTo(deptHead.Email, subject2, body2);
                        }

                        if (sessionUser.Role == Role.DEPT_HEAD || sessionUser.Role == Role.DEPT_ACT_HEAD)
                        {
                            RequisationForm form = itemServices.GetRequisationFormbyId(requisationForm.Id);
                            List<StorePerson> storePersonList = storepersonServices.GetStorePerson();
                            foreach (StorePerson s in storePersonList)
                            {
                                String subject2 = "Requisition Form " + form.RequestNumber + ": Pending";
                                String body2 = "There is a requisition form <a href='https://127.0.0.1:44366/Account/Login' >" + form.RequestNumber + "</a> pending for retrieval.";
                                emailServices.SentEmailTo(s.Email, subject2, body2);
                            }
                            return RedirectToAction("ViewPreviousReqForm", "Employee");
                        }
                        return RedirectToAction("PendingRequestList", "Employee");
                    }
                }
                requisitionViewModel.RequestCode = "";
                requisitionViewModel.DeleteCode = "";
                ModelState.Clear();

            }
            else
            {
                var reqList = new Collection<RequestItem>();
                if (requisitionViewModel.RequestItems != null)
                {
                    reqList = requisitionViewModel.RequestItems;
                }

                requisitionViewModel.RequestItems = reqList;

                foreach (var rItem in reqList)
                {
                    rItem.Item = itemServices.GetItem(rItem.Item.ItemCode);
                }
                requisitionViewModel.RequestCode = "";
                requisitionViewModel.DeleteCode = "";
                requisitionViewModel.RequestItems = reqList;
            }
            return View("RequestForm", requisitionViewModel);
        }

        [Authorize(Roles = "DEPT_REP,EMPLOYEE")]
        public ActionResult PendingRequestList()
        {
            var pendingRequestViewModel = new PendingRequestViewModel();
            var sessionUser = (Employee)Session["usersession"];
            sessionUser = employeeServices.GetEmployee(sessionUser.UserId);
            List<RequisationForm> requisationForms;
            if (sessionUser.Role == Role.EMPLOYEE)
            {
                requisationForms = itemServices.GetAllPendingRequisationFormsbyEmpID(sessionUser.UserId);
            }
            else
            {
                requisationForms = itemServices.GetAllPendingRequisationFormsbyDep(sessionUser.DepartmentCode);
            }

            pendingRequestViewModel.RequisationForms = requisationForms;
            return View(pendingRequestViewModel);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,EMPLOYEE,DEPT_ACT_HEAD")]
        public ActionResult EditRequstionForm(int Id)
        {
            RequisitionViewModel requisitionViewModel = new RequisitionViewModel();
            var RequestForm = itemServices.GetRequisationFormbyId(Id);
            requisitionViewModel.RequestItems = RequestForm.RequestItems;
            requisitionViewModel.RequisationFormID = Id;
            return View("RequestForm", requisitionViewModel);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,EMPLOYEE,DEPT_ACT_HEAD")]
        public ActionResult ItemDetailsfromForm(int Id)
        {
            var RequestForm = itemServices.GetRequisationFormbyId(Id);
            return PartialView("ItemDetailsfromForm", RequestForm);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,EMPLOYEE,DEPT_ACT_HEAD")]
        public ActionResult DeleteRequstionForm(int Id)
        {
            itemServices.DeleteRequstionForm(Id);
            return RedirectToAction("PendingRequestList");
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_ACT_HEAD")]
        public ActionResult ApprovalRequestList()
        {
            var pendingRequestViewModel = new PendingRequestViewModel();
            Employee employee = (Employee)Session["usersession"];
            employee = employeeServices.GetEmployee(employee.UserId);
            List<RequisationForm> requisationForms;

            requisationForms = itemServices.GetAllPendingRequisationFormsbyDep(employee.DepartmentCode);

            pendingRequestViewModel.RequisationForms = requisationForms;
            return View(pendingRequestViewModel);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_ACT_HEAD,DEPT_REP")]
        public ActionResult Dashboard()
        {
            var sessionUser = (Employee)Session["usersession"];
            sessionUser = employeeServices.GetEmployee(sessionUser.UserId);
            var requisationFormsCount = itemServices.GetAllPendingRequisationFormsbyDep(sessionUser.DepartmentCode).Count;

            var employeeDashboardViewModel = new EmplyeeDashboardViewModel
            {
                pendingapproval = requisationFormsCount,
                pendingdisbursements = dashboardServices.GetDisbursementListbyDepCode(sessionUser.DepartmentCode)
            };

            return View(employeeDashboardViewModel);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_ACT_HEAD")]
        public ActionResult ApproveReqForm(string comments, int id)
        {
            Employee employee = (Employee)Session["usersession"];
            employee = employeeServices.GetEmployee(employee.UserId);
            FormStatus formStatus = FormStatus.PENDING_ADDITION;
            itemServices.ChangeStatusWithComments(employee.UserId, id, formStatus, comments);

            var reqForm = itemServices.GetRequisationFormbyId(id);
            Employee emp = employeeServices.GetEmployeeByReqForm(reqForm);
            String subject1 = "Requisition Form " + reqForm.RequestNumber + ": Approved";
            String body1 = "Your requested form <a href='https://127.0.0.1:44366/Account/Login' >" + reqForm.RequestNumber + "</a> is approved.";
            emailServices.SentEmailTo(emp.Email, subject1, body1);

            //send Email to StoreClerk
            List<StorePerson> storePersonList = storepersonServices.GetStorePerson();
            foreach (StorePerson s in storePersonList)
            {
                String subject2 = "Requisition Form " + reqForm.RequestNumber + ": Pending";
                String body2 = "There is a requisition form <a href='https://127.0.0.1:44366/Account/Login' >" + reqForm.RequestNumber + "</a> pending for retrieval.";
                emailServices.SentEmailTo(s.Email, subject2, body2);
            }
            return RedirectToAction("ApprovalRequestList");
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_ACT_HEAD")]
        public ActionResult RejectReqForm(string comments, int id)
        {
            Employee employee = (Employee)Session["usersession"];
            employee = employeeServices.GetEmployee(employee.UserId);
            FormStatus formStatus = FormStatus.REJECTED;
            itemServices.ChangeStatusWithComments(employee.UserId, id, formStatus, comments);

            //send Email to Employee
            var reqForm = itemServices.GetRequisationFormbyId(id);
            Employee emp = employeeServices.GetEmployeeByReqForm(reqForm);
            String subject1 = "Requisition Form " + reqForm.RequestNumber + ": Rejected";
            String body1 = "Your requested form <a href='https://127.0.0.1:44366/Account/Login' >" + reqForm.RequestNumber + "</a> is rejected.";
            emailServices.SentEmailTo(emp.Email, subject1, body1);

            return RedirectToAction("ApprovalRequestList");
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,EMPLOYEE,DEPT_ACT_HEAD")]
        public ActionResult ViewPreviousReqForm()
        {
            var pendingRequestViewModel = new PendingRequestViewModel();
            Employee employee = (Employee)Session["usersession"];
            employee = employeeServices.GetEmployee(employee.UserId);
            List<RequisationForm> requisationForms;
            if (employee.Role == Role.EMPLOYEE)
            {
                requisationForms = itemServices.GetAllOtherRequisationFormsbyEmpID(employee.UserId);
            }
            else
            {
                requisationForms = itemServices.GetAllOtherPendingRequisationFormsbyDep(employee.DepartmentCode);
            }

            pendingRequestViewModel.RequisationForms = requisationForms;
            return View(pendingRequestViewModel);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,EMPLOYEE,DEPT_ACT_HEAD")]
        public ActionResult ViewCatalogue()
        {
            var itemList = itemServices.GetActiveItems();
            ItemViewModel itemViewModel = new ItemViewModel
            {
                ItemList = itemList
            };
            return View(itemViewModel);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,DEPT_ACT_HEAD")]
        public ActionResult GetDepDisbursementListByDepartment()
        {
            Employee employee = (Employee)Session["usersession"];
            employee = employeeServices.GetEmployee(employee.UserId);
            List<DepDisbursementList> depDisbursementLists = itemServices.GetDepDisbursementListByDepartment(employee.Department);
            return View(depDisbursementLists);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,DEPT_ACT_HEAD")]
        public ActionResult GetDisbursementDetailsByDepartment(int Id)
        {
            DepDisbursementList depDisbursementList = itemServices.GetDepDisbursementById(Id);
            return PartialView("GetDisbursementDetailsByDepartment", depDisbursementList);
        }

        [Authorize(Roles = "DEPT_HEAD,DEPT_REP,DEPT_ACT_HEAD")]
        public ActionResult GetDepDisbursementHistoryByDepartment()
        {
            Employee employee = (Employee)Session["usersession"];
            employee = employeeServices.GetEmployee(employee.UserId);
            List<DepDisbursementList> depDisbursementLists = itemServices.GetDepDisbursementHistoryByDepartment(employee.Department);
            return View(depDisbursementLists);
        }


    }
}
