using SSIS.Enums;
using SSIS.Models;
using SSIS.Models.Util_Models;
using SSIS.Services;
using SSIS.View_Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;

namespace SSIS.Controllers.api
{
    /*
      *Coded by
      * Arun Nishanthan Anbalagan
      * Aye Pwint Phyu
      * May Thandar Theint Aung
      */
    public class ItemController : ApiController
    {
        private SSISDbContext dbContext = new SSISDbContext();

        private ItemServices itemServices;
        private EmployeeServices employeeServices;
        private DepartmentServices departmentServices;
        private EmailServices emailServices;

        public ItemController()
        {
            itemServices = new ItemServices(dbContext);
            employeeServices = new EmployeeServices(dbContext);
            departmentServices = new DepartmentServices(dbContext);
            emailServices = new EmailServices();
        }

        [HttpGet]
        public IHttpActionResult SearchItems(string value)
        {
            if (value != null)
            {
                value = value.ToUpper();
                List<Item> itemList = new List<Item>();
                foreach (var item in itemServices.GetActiveItems())
                {
                    if (item.Description.ToUpper().Contains(value) || item.Category.ToUpper().Contains(value) ||
                        item.ItemCode.ToUpper().Contains(value))
                    {
                        itemList.Add(item);
                    }
                }

                return Ok(itemList);
            }
            else
            {
                List<Item> itemList = itemServices.GetActiveItems();
                return Ok(itemList);
            }
        }

        [HttpGet]
        public IHttpActionResult SearchItemsforPO(string value)
        {
            if (value != null)
            {
                value = value.ToUpper();
                List<Item> itemList = new List<Item>();
                foreach (var item in itemServices.GetItems())
                {
                    if (item.Description.ToUpper().Contains(value) || item.Category.ToUpper().Contains(value) ||
                        item.ItemCode.ToUpper().Contains(value))
                    {
                        itemList.Add(item);
                    }
                }

                return Ok(itemList);
            }
            else
            {
                List<Item> itemList = itemServices.GetActiveItems();
                return Ok(itemList);
            }
        }


        [HttpGet]
        public IHttpActionResult ApprovalRequestList(int empId)
        {
            var pendingRequestViewModel = new PendingRequestViewModel();
            var employee = employeeServices.GetEmployee(empId);
            List<RequisationForm> requisationForms =
                itemServices.GetAllPendingRequisationFormsbyDepAPI(employee.DepartmentCode);
            pendingRequestViewModel.RequisationForms = requisationForms;
            return Ok(pendingRequestViewModel);
        }

        [HttpPost]
        public IHttpActionResult AddRequisitionForm(RequestForm requestForm)
        {
            itemServices.SaveReqFormfromAndroid(requestForm);
            return Ok();
        }


        [HttpGet]
        public IHttpActionResult RetrievalLists()
        {
            List<Retrieval1> tempRetrievalList = new List<Retrieval1>();
            RetrievalList1 retrievalList = new RetrievalList1();
            List<RetrievalList1> tempRetrievalLists = new List<RetrievalList1>();
            RetrievalListViewModel1 retrievalListViewModel = new RetrievalListViewModel1();
            string reqNo = "";

            //List<RequestItem> distinctItemList = new List<RequestItem>();
            List<string> distItemLists = new List<string>();
            List<RequisationForm> requisationFormList = itemServices.GetFormByStatusRetrievel();

            //find distinct item from requisition form
            foreach (RequisationForm rf in requisationFormList)
            {
                reqNo += rf.Id + ",";
                foreach (RequestItem ri in rf.RequestItems)
                {
                    if (!distItemLists.Contains(ri.Item.Description))
                    {
                        distItemLists.Add(ri.Item.Description);
                    }
                }
            }

            //create list group by item and add to view model
            int currentQty = 0;
            for (int i = 0; i < distItemLists.Count; i++)
            {
                int count = 0;
                string item1 = distItemLists[i];
                tempRetrievalList = new List<Retrieval1>();
                Department department = new Department();
                foreach (RequisationForm rf in requisationFormList)
                {
                    foreach (RequestItem ri in rf.RequestItems)
                    {
                        string item2 = ri.Item.Description;
                        if (item1 == item2)
                        {
                            if (tempRetrievalList.Count == 0) //if empty list( 1st retrieval)
                            {
                                var retrieval = new Retrieval1
                                {
                                    DepartmentCode = rf.RequestedBy.Department.DepartmentCode,
                                    Needed = ri.Quantity
                                };
                                count += ri.Quantity;
                                retrieval.Actual = 0;
                                currentQty = ri.Item.CurrentQuantity;

                                tempRetrievalList.Add(retrieval);
                            }
                            else if (tempRetrievalList.Count != 0) //check dep
                            {
                                var retrivalFromtemp = tempRetrievalList.Find(m =>
                                    m.DepartmentCode == rf.RequestedBy.Department.DepartmentCode);
                                if (retrivalFromtemp != null)
                                {
                                    retrivalFromtemp.Needed += ri.Quantity;
                                    count += ri.Quantity;
                                }
                                else
                                {
                                    var retrieval = new Retrieval1
                                    {
                                        DepartmentCode = rf.RequestedBy.Department.DepartmentCode,
                                        Needed = ri.Quantity
                                    };
                                    count += ri.Quantity;
                                    retrieval.Actual = 0;
                                    currentQty = ri.Item.CurrentQuantity;

                                    tempRetrievalList.Add(retrieval);
                                }
                            }
                        }
                    }
                }

                retrievalList = new RetrievalList1
                {
                    RetrievedItem = distItemLists[i],
                    Retrievals = tempRetrievalList,
                    Needed = count
                };
                //check Qty
                if (currentQty > retrievalList.Needed)
                {
                    retrievalList.Retrieved = retrievalList.Needed;
                    for (int j = 0; j < retrievalList.Retrievals.Count; j++)
                    {
                        retrievalList.Retrievals[j].Actual = retrievalList.Retrievals[j].Needed;
                    }
                }
                else
                {
                    retrievalList.Retrieved = currentQty;
                }

                tempRetrievalLists.Add(retrievalList);
            }

            retrievalListViewModel.RetrivalLists = tempRetrievalLists;
            retrievalListViewModel.RequisationFormsID = reqNo;
            return Ok(retrievalListViewModel);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult GenerateDisbursementLists(RetrievalListViewModel1 retrievalListViewModel1)
        {

            //find distinct department from requisition forms
            var departments = new List<Department>();
            foreach (var retrievalList in retrievalListViewModel1.RetrivalLists)
            {
                foreach (var retrieval in retrievalList.Retrievals)
                {
                    Department department = null;
                    if (departments.Count != 0)
                    {
                        department = departments.Find(m => m.DepartmentCode == retrieval.DepartmentCode);
                    }

                    if (department == null)
                    {
                        department = departmentServices.GetDepartment(retrieval.DepartmentCode);
                        departments.Add(department);
                    }
                }
            }

            //prepare data to save in db
            foreach (var depart in departments)
            {
                DepDisbursementList depDisbursementList = new DepDisbursementList
                {
                    Department = depart,
                    DisburseDate = DateTime.MinValue,
                    DisbursementStatus = DisbursementStatus.WAITING_FOR_ACK,
                    DisburseItems = new Collection<DisburseItem>()
                };

                List<DisburseItem> disburseItems = new List<DisburseItem>();

                foreach (var retrivalList in retrievalListViewModel1.RetrivalLists)
                {
                    foreach (var retrival in retrivalList.Retrievals)
                    {
                        if (retrival.DepartmentCode == depart.DepartmentCode)
                        {
                            DisburseItem disburseItem = new DisburseItem
                            {
                                RequestItem = itemServices.GetItembyDes(retrivalList.RetrievedItem),
                                RequestedQuantity = retrival.Needed,
                                RetrivedQuantity = retrival.Actual,
                                DisbursedQuantity = retrival.Actual
                            };
                            disburseItems.Add(disburseItem);

                            //Save each disburse item
                            itemServices.SaveDisbursedItem(disburseItem);
                        }
                    }
                }

                depDisbursementList.DisburseItems = new Collection<DisburseItem>(disburseItems);
                //Save DisubursementList by Department
                itemServices.SaveDepDisbursedList(depDisbursementList);

                //Change form status "Pending Retrieval" to "Pending Disbursement"
                string[] formNums = retrievalListViewModel1.RequisationFormsID.Split(',');

                for (int i = 0; i < formNums.Length - 1; i++)
                {
                    itemServices.ChangeStatusToDisbursement(int.Parse(formNums[i]), FormStatus.PENDING_DISBURSEMENT);
                }
            }
            foreach (Department department in departments)
            {
                DepDisbursementList depDibursement =
                    itemServices.GetDepDisbursementListsByDepCode(department.DepartmentCode);
                Employee depRep = employeeServices.GetEmployee(depDibursement.Department.DepartmentRepresentative.UserId);
                string subject = "Disbursement Number " + depDibursement.DepDisbursementListNumber + ": Pending";
                string body =
                    "Your requisition forms (DisbursmentNo: <a href='https://127.0.0.1:44366/Account/Login' >" +
                    depDibursement.DepDisbursementListNumber + "</a>) are pending in disbursement List.";
                //insert table 
                string textBody = body + "<br><br><table border="
                                       + 1 +
                                       " cellpadding="
                                       + 0 +
                                       " cellspacing="
                                       + 0 +
                                       " width = "
                                       + 400 +
                                       ">" +
                                       "<thead>" +
                                       "<tr>" +
                                       "<th>Item Description</th>" +
                                       "<th>Requested Quantity</th>" +
                                       "<th>Retrieved Quantity</th>" +
                                       "</tr>" +
                                       "</thead>" +
                                       "<tbody>";
                for (var i = 0; i < depDibursement.DisburseItems.Count; i++)
                {

                    textBody += "<tr>" +
                                "<td>" + depDibursement.DisburseItems[i].RequestItem.Description + "</td>" +
                                "<td>" + depDibursement.DisburseItems[i].RequestedQuantity + "</td>" +
                                "<td>" + depDibursement.DisburseItems[i].RetrivedQuantity + "</td>" +
                                "</tr>";

                }

                textBody += "</table>";
                emailServices.SentEmailTo(depRep.Email, subject, textBody);
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult RequisitionList()
        {
            var requisitionListViewModel = new RequisitionDepartmentViewModel();
            var requisitionLists = new List<RequisitionByDepartmentItem>();
            var reqList = itemServices.GetAdditionStatusRequisationForms();
            foreach (var item in reqList)
            {
                RequisitionByDepartmentItem requisitionByDepartmentItem = null;
                if (requisitionLists.Count != 0)
                {
                    requisitionByDepartmentItem =
                        requisitionLists.Find(m => m.Department.DepartmentCode == item.RequestedBy.DepartmentCode);
                }

                if (requisitionByDepartmentItem == null)
                {
                    var requisitionByDepartmentItm = new RequisitionByDepartmentItem
                    {
                        Department = item.RequestedBy.Department,
                        LastUpdated = item.RequestedDate,
                        RequestItems = item.RequestItems.ToList()
                    };
                    requisitionLists.Add(requisitionByDepartmentItm);
                }
                else
                {
                    if (requisitionByDepartmentItem.LastUpdated < item.RequestedDate)
                    {
                        requisitionByDepartmentItem.LastUpdated = item.RequestedDate;
                    }

                    foreach (var requestItemInList in item.RequestItems)
                    {
                        RequestItem requestItem = null;

                        if (requisitionByDepartmentItem.RequestItems.Count != 0)
                        {
                            requestItem = requisitionByDepartmentItem.RequestItems.Find(m =>
                                m.Item.ItemCode == requestItemInList.Item.ItemCode);
                        }

                        if (requestItem == null)
                        {
                            requisitionByDepartmentItem.RequestItems.Add(requestItemInList);
                        }
                        else
                        {
                            requestItem.Quantity += requestItemInList.Quantity;
                        }
                    }
                }
            }

            requisitionListViewModel.RequisitionLists = requisitionLists;
            return Ok(requisitionListViewModel);
        }

        [HttpPost]
        public IHttpActionResult ChangeStatusToRetrivalByDep(string depCode)
        {
            itemServices.ChangeStatusToRetrivalByDep(depCode);
            return Ok();
        }


        [System.Web.Http.HttpGet]
        public IHttpActionResult DeptDisburmentLists()
        {
            List<DepDisbursementList> tempdeptDList;
            tempdeptDList = itemServices.GetDepDisbursementLists();

            DeptDisbursementViewModel deptDList = new DeptDisbursementViewModel
            {
                DepDisbursementLists = tempdeptDList
            };

            return Ok(deptDList);
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult DeptDisburmentListsbyId(int empId)
        {
            Employee employeeDep = employeeServices.GetEmployee(empId);
            List<DepDisbursementList> deplist = new List<DepDisbursementList>();
            var tempdeptDList = itemServices.GetDepDisbursementLists();

            foreach (var list in tempdeptDList)
            {
                if (list.Department.DepartmentCode == employeeDep.DepartmentCode)
                {
                    deplist.Add(list);
                }
            }

            DeptDisbursementViewModel deptDList = new DeptDisbursementViewModel
            {
                DepDisbursementLists = deplist
            };

            return Ok(deptDList);
        }

        [HttpPost]
        public IHttpActionResult DeptDisbuseementUpdateFromApp(DisburseRequest disburseRequest)
        {
            itemServices.AcknowledgefromAndroid(disburseRequest);
            //send OTP with mail code is here
            DepDisbursementList otpDepDisbursementList =
                itemServices.GetDepDisbursementById(disburseRequest.Id);
            string strOTP = otpDepDisbursementList.OTP;

            var dep = departmentServices.GetDepartment(otpDepDisbursementList.Department.DepartmentCode);
            var deptRep = employeeServices.GetEmployee(dep.DepartmentRepresentative.UserId);
            string subject = "OTP Code for DisbursementNo." + otpDepDisbursementList.DepDisbursementListNumber;
            string body =
                "OTP code for (DisbursmentNo: <a href='https://127.0.0.1:44366/Account/Login'>" +
                otpDepDisbursementList.DepDisbursementListNumber + "</a>) is <b>" + strOTP + "</b>.";
            emailServices.SentEmailTo(deptRep.Email, subject, body);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult ValidateOTP(int Id, string OTP)
        {
            int status = 0;
            DepDisbursementList depDisbursement = itemServices.GetDepDisbursementById(Id);

            if (depDisbursement.OTP == OTP.ToString() || depDisbursement.OTP.Equals(OTP))
            {
                Department department = depDisbursement.Department;
                Employee depRep = employeeServices.GetEmployee(department.DepartmentRepresentative.UserId);
                string subject1 = "OTP Authentication: Success";
                string body1 = "Your OTP verification is successful.";
                emailServices.SentEmailTo(depRep.Email, subject1, body1);
                itemServices.ChangeDisbursementForOTP(depDisbursement.DepDisbursementListId);
                status = 1;
            }
            return Ok(status);
        }

        [HttpGet]
        public IHttpActionResult ResentOTP(int Id)
        {
            DepDisbursementList depDisbursementList = itemServices.SaveResentOTP(Id);
            string newOTP = depDisbursementList.OTP;
            Department department = depDisbursementList.Department;
            //send OTP with mail code is here
            Employee depRep = employeeServices.GetEmployee(department.DepartmentRepresentative.UserId);
            string subject = "Resend OTP Code " + depDisbursementList.DepDisbursementListNumber;
            string body =
                "Resend OTP code for (DisbursmentNo: <a href='https://127.0.0.1:44366/Account/Login' >" +
                depDisbursementList.DepDisbursementListNumber + "</a>) is <b>" + newOTP + "</b>.";
            emailServices.SentEmailTo(depRep.Email, subject, body);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult CancelDisbursementList(int Id)
        {
            itemServices.CancelDisbursementList(Id);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GenerateRequisation(int id)
        {
            DepDisbursementList disbursementList = itemServices.GetDepDisbursementById(id);
            if (!disbursementList.isGenerated)
            {
                Collection<DisburseItem> disburseItems = disbursementList.DisburseItems;
                RequisationForm newRequisationForm = new RequisationForm();
                Collection<RequestItem> newRequestItems = new Collection<RequestItem>();
                foreach (DisburseItem item in disburseItems)
                {
                    if (item.DisbursedQuantity < item.RequestedQuantity)
                    {
                        RequestItem requestItem = new RequestItem
                        {
                            Item = item.RequestItem,
                            Quantity = item.RequestedQuantity - item.DisbursedQuantity
                        };
                        requestItem = itemServices.SaveRequestItem(requestItem);
                        newRequestItems.Add(requestItem);
                    }
                }
                newRequisationForm.FormStatus = SSIS.Enums.FormStatus.PENDING_ADDITION;
                newRequisationForm.RequestedBy = disbursementList.Department.DepartmentHead;
                newRequisationForm.HandeledBy = disbursementList.Department.DepartmentHead;
                newRequisationForm.RequestItems = newRequestItems;
                newRequisationForm.HandeledDate = DateTime.Now;
                newRequisationForm.RequestedDate = DateTime.Now;
                newRequisationForm.Comments = "Auto Generated";
                var requisationFormDb = itemServices.SaveRequisationForm(newRequisationForm);
                int i = itemServices.GenerateRequisationForm(id);
                if (i > 0)
                {
                    // sent mail to head for new created form
                    RequisationForm form = requisationFormDb;
                    string subject = "New Requisition form created";
                    string body =
                        "New Requisition Form (No: <a href='https://127.0.0.1:44366/Account/Login' >" +
                        form.RequestNumber + "</a>) is created.";
                    emailServices.SentEmailTo(form.HandeledBy.Email, subject, body);

                }
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult RemoveDisbursementItem(int Id)
        {


            var disburseItem = itemServices.GetDisburseItembyId(Id);
            itemServices.RemoveDisbursementItems(Id, disburseItem.DepDisbursementList.DepDisbursementListId);
            return Ok();
        }
    }
}
