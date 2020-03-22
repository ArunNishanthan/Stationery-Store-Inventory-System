using Newtonsoft.Json;
using SSIS.Enum;
using SSIS.Enums;
using SSIS.Models;
using SSIS.Security.Filter;
using SSIS.Services;
using SSIS.View_Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace SSIS.Controllers
{
    /*
     *Coded by
     * Arun Nishanthan Anbalagan
     * Aye Pwint Phyu
     * Hay Mun Linn
     * May Thandar Theint Aung
     * Raja Sudalaimuthu Padma
     * Samuel Wang Yu Rong
     * Yao Hai
     * Tang Zi Jian
     */
    [Authorize(Roles = "STORE_CLERK, STORE_SUPERVISOR, STORE_MANAGER")]
    [SessionCheck]
    public class StoreClerkController : Controller
    {
        private SSISDbContext dbContext = new SSISDbContext();

        private ItemServices itemServices;
        private DepartmentServices departmentServices;
        private ItemSupplierServices itemSupplierService;
        private StorePersonServices storepersonServices;
        private EmailServices emailServices;
        private EmployeeServices employeeServices;
        private DashboardServices dashboardServices;
        private ItemSupplierServices itemSupplierServices;
        public StoreClerkController()
        {
            itemServices = new ItemServices(dbContext);
            departmentServices = new DepartmentServices(dbContext);
            itemSupplierService = new ItemSupplierServices(dbContext);
            storepersonServices = new StorePersonServices(dbContext);
            emailServices = new EmailServices();
            employeeServices = new EmployeeServices(dbContext);
            dashboardServices = new DashboardServices(dbContext);
            itemSupplierServices = new ItemSupplierServices(dbContext);
        }

        public ActionResult Dashboard()
        {
            var storeDashboardViewModel = new StoreDashboardViewModel
            {
                lowStockItemCount = dashboardServices.GetLowStockItemsCount(),
                disbursementlist = dashboardServices.GetPendingDisbursementList(),
                pODelivery = dashboardServices.GetPendingPurOrderDelivery(),
                retrievalList = dashboardServices.GetPendingRetrievalList()
            };
            return View(storeDashboardViewModel);
        }

        public ActionResult RequisitionList()
        {
            var requisitionListViewModel = new RequisitionListViewModel();
            var requisitionLists = new List<RequisitionByDepartment>();
            var reqList = itemServices.GetAdditionStatusRequisationForms();
            foreach (var item in reqList)
            {
                RequisitionByDepartment RequisByDepartment = null;
                if (requisitionLists.Count != 0)
                {
                    RequisByDepartment =
                        requisitionLists.Find(m => m.Department.DepartmentCode == item.RequestedBy.DepartmentCode);
                }

                if (RequisByDepartment == null)
                {
                    var requisitionByDepartment = new RequisitionByDepartment
                    {
                        Department = item.RequestedBy.Department,
                        LastUpdated = item.RequestedDate
                    };
                    requisitionLists.Add(requisitionByDepartment);
                }
                else
                {
                    if (RequisByDepartment.LastUpdated < item.RequestedDate)
                    {
                        RequisByDepartment.LastUpdated = item.RequestedDate;
                    }
                }
            }

            requisitionListViewModel.RequisitionLists = requisitionLists;
            return View(requisitionListViewModel);
        }

        public ActionResult ItemDetailsbyDep(string Id)
        {
            var reqForms = itemServices.GetAllPendingAdditionRequisationFormsbyDep(Id);
            var requestItems = new List<RequestItem>();

            foreach (var reqForm in reqForms)
            {
                foreach (var reqItem in reqForm.RequestItems)
                {
                    RequestItem requestItem = null;
                    if (requestItems.Count != 0)
                    {
                        requestItem = requestItems.Find(m => m.Item.ItemCode == reqItem.Item.ItemCode);
                    }

                    if (requestItem != null)
                    {
                        requestItem.Quantity += reqItem.Quantity;
                    }
                    else
                    {
                        requestItem = new RequestItem
                        {
                            Quantity = reqItem.Quantity,
                            Item = reqItem.Item
                        };
                        requestItems.Add(requestItem);
                    }
                }
            }

            var requisitionViewModel = new RequisitionViewModel
            {
                RequestItems = new Collection<RequestItem>(requestItems)
            };
            ViewBag.DepName = reqForms[0].RequestedBy.Department.DepartmentName;
            return PartialView(requisitionViewModel);
        }

        public ActionResult ChangeStatusToRetrivalByDep(string depCode)
        {
            itemServices.ChangeStatusToRetrivalByDep(depCode);
            return RedirectToAction("RequisitionList");
        }

        public ActionResult RetrievalLists(RetrievalListViewModel retrievalListViewModel1)
        {
            List<Retrieval> tempRetrievalList = new List<Retrieval>();
            RetrievalList retrievalList = new RetrievalList();
            List<RetrievalList> tempRetrievalLists = new List<RetrievalList>();
            RetrievalListViewModel retrievalListViewModel = new RetrievalListViewModel
            {
                ErrorFlag = retrievalListViewModel1.ErrorFlag
            };
            string reqNo = "";

            List<RequestItem> distinctItemList = new List<RequestItem>();
            List<Item> distItemLists = new List<Item>();
            List<RequisationForm> requisationFormList = itemServices.GetFormByStatusRetrievel();

            //find distinct item from requisition form
            foreach (RequisationForm rf in requisationFormList)
            {
                reqNo += rf.Id + ",";
                foreach (RequestItem ri in rf.RequestItems)
                {
                    if (!distItemLists.Contains(ri.Item))
                    {
                        distItemLists.Add(ri.Item);
                    }
                }
            }

            //create list group by item and add to view model
            int currentQty = 0;
            for (int i = 0; i < distItemLists.Count; i++)
            {
                int count = 0;
                Item item1 = distItemLists[i];
                tempRetrievalList = new List<Retrieval>();
                Department department = new Department();
                foreach (RequisationForm rf in requisationFormList)
                {
                    foreach (RequestItem ri in rf.RequestItems)
                    {
                        string item2 = ri.Item.Description;
                        if (item1.Description == item2)
                        {
                            if (tempRetrievalList.Count == 0) //if empty list( 1st retrieval)
                            {
                                var retrieval = new Retrieval
                                {
                                    Department = rf.RequestedBy.Department,
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
                                    m.Department.DepartmentCode == rf.RequestedBy.Department.DepartmentCode);
                                if (retrivalFromtemp != null)
                                {
                                    retrivalFromtemp.Needed += ri.Quantity;
                                    count += ri.Quantity;
                                }
                                else
                                {
                                    var retrieval = new Retrieval
                                    {
                                        Department = rf.RequestedBy.Department,
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

                retrievalList = new RetrievalList
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
                    if (currentQty < 0)
                    {
                        retrievalList.Retrieved = 0;
                    }
                    else
                    {
                        retrievalList.Retrieved = currentQty;
                    }

                }

                tempRetrievalLists.Add(retrievalList);
            }

            retrievalListViewModel.RetrivalLists = tempRetrievalLists;
            retrievalListViewModel.RequisationFormsID = reqNo;
            return View(retrievalListViewModel);
        }

        [HttpPost]
        public ActionResult GenerateDisbursementListbyDept(RetrievalListViewModel retrievalListViewModel)
        {
            //check Retrieved and total Actual 
            RetrievalListViewModel retrievalListVM = retrievalListViewModel;

            foreach (var retrievalList in retrievalListVM.RetrivalLists)
            {
                int totalActualQty = 0;
                foreach (var retrieval in retrievalList.Retrievals)
                {
                    totalActualQty += retrieval.Actual;
                }
                if (retrievalList.Retrieved < totalActualQty)
                {
                    retrievalListVM.ErrorFlag = true;
                    return RedirectToAction("RetrievalLists", retrievalListVM);
                }
            }
            //find distinct department from requisition forms
            var departments = new List<Department>();
            foreach (var retrievalList in retrievalListViewModel.RetrivalLists)
            {
                foreach (var retrieval in retrievalList.Retrievals)
                {
                    Department department = null;
                    if (departments.Count != 0)
                    {
                        department = departments.Find(m => m.DepartmentCode == retrieval.Department.DepartmentCode);
                    }

                    if (department == null)
                    {
                        department = departmentServices.GetDepartment(retrieval.Department.DepartmentCode);
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

                foreach (var retrivalList in retrievalListViewModel.RetrivalLists)
                {
                    foreach (var retrival in retrivalList.Retrievals)
                    {
                        if (retrival.Department.DepartmentCode == depart.DepartmentCode)
                        {
                            DisburseItem disburseItem = new DisburseItem
                            {
                                RequestItem = itemServices.GetItem(retrivalList.RetrievedItem.ItemCode),

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
                string[] formNums = retrievalListViewModel.RequisationFormsID.Split(',');

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

            return RedirectToAction("RequisitionList", "StoreClerk");
        }

        public ActionResult StockCard(string id)
        {
            var stockCards = itemServices.GetStockCards(id);
            StockCardViewModel stockCardViewModel = new StockCardViewModel
            {
                StockCards = stockCards
            };
            if (stockCardViewModel.StockCards.Count == 0)
            {
                return PartialView("NoStockCard");
            }
            return PartialView(stockCardViewModel);
        }

        public ActionResult CreateVoucher(string id)
        {
            var item = itemServices.GetItem(id);
            Voucher voucher = new Voucher
            {
                Item = item,
                Quantity = 1
            };
            return View(voucher);
        }

        [HttpPost]
        public ActionResult CheckVoucher(Voucher voucher)
        {
            var item = itemServices.GetItem(voucher.Item.ItemCode);
            voucher.Item = item;
            voucher.CreatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                itemServices.SaveVoucher(voucher);
                return RedirectToAction("QuantityRecommendation", "StoreClerk");
            }
            return View("createVoucher", voucher);
        }

        public ActionResult GetDepDisbursementList()
        {
            List<DepDisbursementList> depDisbursementLists = itemServices.GetDepDisbursementLists();
            return View(depDisbursementLists);
        }

        public ActionResult GetDepDisbursementHistory()
        {
            List<DepDisbursementList> depDisbursementLists = itemServices.GetDepDisbursementHistory();
            return View(depDisbursementLists);
        }

        [HttpGet]
        public void RemoveDisbursementItem(int Id, int DepId)
        {
            itemServices.RemoveDisbursementItems(Id, DepId);
        }

        [HttpPost]
        public ActionResult AnknowledgeDisbursementItems(DepDisbursementList paraDisbursementList)
        {
            itemServices.AnknowledgeDisbursementItems(paraDisbursementList);
            //send OTP with mail code is here
            DepDisbursementList otpDepDisbursementList = itemServices.GetDepDisbursementById(paraDisbursementList.DepDisbursementListId);
            string strOTP = otpDepDisbursementList.OTP;

            Employee deptRep = employeeServices.GetEmployee(otpDepDisbursementList.Department.DepartmentRepresentative.UserId);

            string subject = "OTP Code for DisbursementNo." + otpDepDisbursementList.DepDisbursementListNumber;
            string body =
                "OTP code for (DisbursmentNo: <a href='https://127.0.0.1:44366/Account/Login'>" +
                otpDepDisbursementList.DepDisbursementListNumber + "</a>) is <b>" + strOTP + "</b>.";
            emailServices.SentEmailTo(deptRep.Email, subject, body);
            return RedirectToAction("GetDepDisbursementList");
        }

        public ActionResult Details(int Id)
        {
            DepDisbursementList depDisbursementList = itemServices.GetDepDisbursementById(Id);
            return PartialView("Details", depDisbursementList);
        }

        public ActionResult CancelDisbursementList(int Id)
        {
            itemServices.CancelDisbursementList(Id);
            return RedirectToAction("GetDepDisbursementList");
        }

        public ActionResult GenerateRequisation(int id)
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
            return RedirectToAction("GetDepDisbursementList");
        }

        public ActionResult EnterOTP(int Id)
        {
            DepDisbursementList disbursementList = itemServices.GetDepDisbursementById(Id);
            disbursementList.OTP = "";
            return PartialView("EnterOTP", disbursementList);
        }

        [HttpGet]
        public bool CheckOTP(int Id, int OTP)
        {
            DepDisbursementList depDisbursement = itemServices.GetDepDisbursementById(Id);
            if (depDisbursement.OTP == OTP.ToString() || depDisbursement.OTP.Equals(OTP))
            {
                return true;
            }
            return false;
        }

        public ActionResult ValidateOTP(int Id)
        {
            DepDisbursementList depDisbursement = itemServices.GetDepDisbursementById(Id);
            Department department = depDisbursement.Department;
            //select dep rep to sent email
            Employee depRep = employeeServices.GetEmployee(department.DepartmentRepresentative.UserId);
            //sent successful msg with email
            string subject1 = "OTP Authentication: Success";
            string body1 = "Your OTP verification is successful.";
            emailServices.SentEmailTo(depRep.Email, subject1, body1);

            itemServices.ChangeDisbursementForOTP(depDisbursement.DepDisbursementListId);
            return RedirectToAction("GetDepDisbursementList");
        }

        [HttpGet]
        public ActionResult ResentOTP(int Id)
        {
            DepDisbursementList depDisbursementList = itemServices.SaveResentOTP(Id);
            string newOTP = depDisbursementList.OTP;
            //send OTP with mail code is here
            Employee deptRep = employeeServices.GetDeptRepbyDepCode(depDisbursementList.Department.DepartmentCode);
            string subject = "Resend OTP Code " + depDisbursementList.DepDisbursementListNumber;
            string body =
                "Resend OTP code for (DisbursmentNo: <a href='https://127.0.0.1:44366/Account/Login' >" +
                depDisbursementList.DepDisbursementListNumber + "</a>) is <b>" + newOTP + "</b>.";
            emailServices.SentEmailTo(deptRep.Email, subject, body);

            return RedirectToAction("GetDepDisbursementList");
        }

        public ActionResult CreatePurchaseOrder()
        {

            var systemGeneratePurchaseOrderViewModel = new SystemGeneratePurchaseOrderViewModel
            {
                PurchaseOrderQuantityList = new List<PurchaseOrderQuantity>()
            };

            ViewData["count"] = 0;
            return View(systemGeneratePurchaseOrderViewModel);
        }

        public ActionResult CheckReqList(SystemGeneratePurchaseOrderViewModel systemGeneratePurchaseOrderViewModel)
        {
            List<ItemSupplier> itemSupplierList = new List<ItemSupplier>();
            if (systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList != null)
            {
                foreach (var purchaseOrder in systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList)
                {
                    purchaseOrder.itemSupplier =
                        itemSupplierService.GetSupplierbyId(purchaseOrder.itemSupplier.ItemSupplierId);
                }
            }
            else
            {
                systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList = new List<PurchaseOrderQuantity>();
            }
            if (systemGeneratePurchaseOrderViewModel.RequestCode != null)
            {
                var itemSupplierListDb = itemSupplierService.GetItemSuppliersbyItem(systemGeneratePurchaseOrderViewModel.RequestCode);
                itemSupplierList.AddRange(itemSupplierListDb);
            }
            if (systemGeneratePurchaseOrderViewModel.DeleteCode != null)
            {
                var itemSupplierListDb = itemSupplierService.GetItemSuppliersbyItem(systemGeneratePurchaseOrderViewModel.DeleteCode);

                foreach (var itemSupplier in itemSupplierListDb)
                {
                    var purchaseOrderQuantity =
                        systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList.Find(m =>
                            m.itemSupplier.ItemSupplierId == itemSupplier.ItemSupplierId);
                    systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList.Remove(purchaseOrderQuantity);
                }

            }
            foreach (var PO in itemSupplierList)
            {
                var purchaseOrderQuantityList = new PurchaseOrderQuantity
                {
                    itemSupplier = PO,
                    Quantity = 0
                };
                systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList.Add(purchaseOrderQuantityList);
            }
            int count = systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList.Select(x => x.itemSupplier.Item).Distinct().Count();
            ViewData["count"] = count;
            ModelState.Clear();
            if (systemGeneratePurchaseOrderViewModel.RequestCode == null && systemGeneratePurchaseOrderViewModel.DeleteCode == null)
            {
                return AddPurchaseOrder(systemGeneratePurchaseOrderViewModel);
            }

            systemGeneratePurchaseOrderViewModel.RequestCode = "";
            systemGeneratePurchaseOrderViewModel.DeleteCode = "";

            return View("createPurchaseOrder", systemGeneratePurchaseOrderViewModel);
        }

        public ActionResult SystemGeneratePurchaseOrder()
        {
            var systemGeneratePurchaseOrderViewModel = new SystemGeneratePurchaseOrderViewModel
            {
                PurchaseOrderQuantityList = new List<PurchaseOrderQuantity>()
            };
            List<ItemSupplier> itemSupplierList = itemSupplierService.GetLowStockItemSuppliers();

            foreach (var PO in itemSupplierList)
            {
                var purchaseOrderQuantityList = new PurchaseOrderQuantity
                {
                    itemSupplier = PO,
                    Quantity = 0
                };
                systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList.Add(purchaseOrderQuantityList);
            }

            int count = systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList.Select(x => x.itemSupplier.Item).Distinct().Count();
            ViewData["count"] = count;

            return View("createPurchaseOrder", systemGeneratePurchaseOrderViewModel);
        }
        [HttpPost]
        public ActionResult AddPurchaseOrder(SystemGeneratePurchaseOrderViewModel systemGeneratePurchaseOrderViewModel)
        {
            var purchaseOrder = new List<PurchaseOrder>();
            foreach (var purchaseOrderQuantity in systemGeneratePurchaseOrderViewModel.PurchaseOrderQuantityList)
            {
                PurchaseOrder pOrder = null;
                if (purchaseOrder.Count != 0)
                {
                    pOrder = purchaseOrder.Find(p => p.Supplier.SupplierCode == purchaseOrderQuantity.itemSupplier.Supplier.SupplierCode);
                }
                if (pOrder != null)
                {
                    if (purchaseOrderQuantity.Quantity != 0)
                    {
                        PurchaseItem pi = new PurchaseItem
                        {
                            Item = itemServices.GetItem(purchaseOrderQuantity.itemSupplier.Item.ItemCode),
                            Quantity = purchaseOrderQuantity.Quantity
                        };
                        pOrder.PurchaseItems.Add(pi);

                        PurchaseItem savedItem = itemServices.SavePurchaseItem(pi);
                    }
                }
                else
                {
                    if (purchaseOrderQuantity.Quantity != 0)
                    {
                        PurchaseOrder po = new PurchaseOrder
                        {
                            PurchaseOrderDate = DateTime.Now,
                            PurchaseOrderStatus = Enums.PurchaseOrderStatus.PENDING,
                            DeliveredDate = DateTime.MinValue,
                            Supplier = itemSupplierService.GetSupplier(purchaseOrderQuantity.itemSupplier.Supplier)
                        };

                        PurchaseItem pi = new PurchaseItem
                        {
                            Item = itemServices.GetItem(purchaseOrderQuantity.itemSupplier.Item.ItemCode),
                            Quantity = purchaseOrderQuantity.Quantity
                        };

                        po.PurchaseItems = new Collection<PurchaseItem>
                        {
                            pi
                        };

                        purchaseOrder.Add(po);

                        PurchaseItem savedItem = itemServices.SavePurchaseItem(pi);
                    }
                }
            }

            foreach (var item in purchaseOrder)
            {
                itemServices.SavePurchaseOrder(item);
            }
            return RedirectToAction("ViewPurchaseOrder");
        }

        public ActionResult ViewPurchaseOrder()
        {
            List<PurchaseOrder> po = itemServices.GetPendingPurchaseOrder();
            return View("ViewPurchaseOrder", po);
        }

        public ActionResult ChangePurchaseOrderStatus(string purchaseOrderId)
        {
            int pId = int.Parse(purchaseOrderId);
            itemServices.UpdatePurchaseOrderStatus(pId);
            itemServices.UpdateItemQty(pId);
            itemServices.AddStockCard(pId);
            return RedirectToAction("deliveryItemList");
        }

        public ActionResult DeliveryItemList()
        {
            List<PurchaseOrder> delieveredpOrderList = itemServices.GetDelieveredPurchaseOrder();
            return View("deliveryItemList", delieveredpOrderList);
        }

        public ActionResult DeliveryItemDetail(int Id)
        {
            var purchaseOrder = itemServices.GetPurchaseOrder(Id);

            return PartialView(purchaseOrder);
        }

        public ActionResult GenerateVoucher()
        {
            VoucherListToGenerateViewModel voucherListViewModel = new VoucherListToGenerateViewModel();
            List<Voucher> vouchers = itemServices.GetVouchers();
            voucherListViewModel.voucherList = vouchers;
            return View(voucherListViewModel);
        }

        public ActionResult CreateVouchers()
        {
            List<Voucher> vouchers = new List<Voucher>();
            List<Item> items = new List<Item>();
            Collection<Voucher> tempVoucherList = new Collection<Voucher>();
            vouchers = itemServices.GetVouchers();

            //find distinct item from vouchers
            Item item = null;
            foreach (Voucher voucher in vouchers)
            {
                if (items.Count != 0)
                {
                    item = items.Find(m => m.ItemCode == voucher.Item.ItemCode);
                }
                if (item == null)
                {
                    items.Add(itemServices.GetItem(voucher.Item.ItemCode));
                }
            }

            int totalQty = 0;
            foreach (Item i in items)
            {
                tempVoucherList = new Collection<Voucher>();
                foreach (Voucher voucher in vouchers)
                {
                    if (i.ItemCode == voucher.Item.ItemCode)
                    {
                        tempVoucherList.Add(voucher);
                        totalQty += voucher.Quantity;
                        itemServices.ChangeVoucherStatus(voucher.Id, VoucherStatus.INACTIVE);
                    }
                }
                AdjustmentVoucher adjVoucher = new AdjustmentVoucher
                {
                    Item = i,
                    AdjustedQuantity = totalQty,
                    RequestedDate = DateTime.Now
                };
                //Session
                User user = (User)Session["usersession"];
                adjVoucher.RequestedBy = storepersonServices.GetStorePerson(user.UserId);
                adjVoucher.IssuedDate = DateTime.MinValue;
                adjVoucher.VoucherStatus = AdjustmentVoucherStatus.PENDING;
                adjVoucher.Vouchers = tempVoucherList;

                //Save voucher
                itemServices.SaveAdjVoucher(adjVoucher);
            }

            return RedirectToAction("RequestedVoucher", "StoreClerk");
        }

        public ActionResult RequestedVoucher()
        {
            List<AdjustmentVoucherList> tempAdjVoucherList = new List<AdjustmentVoucherList>();
            AdjustmentVoucherViewModel adjVoucherViewModel = new AdjustmentVoucherViewModel();
            List<AdjustmentVoucher> adjustmentVouchers = itemServices.GetAdjVouchers(AdjustmentVoucherStatus.PENDING);

            foreach (AdjustmentVoucher voucher in adjustmentVouchers)
            {
                var v = new AdjustmentVoucherList();
                double price = itemServices.GetPriceByItemCode(voucher.Item.ItemCode);
                double adjPrice = price * voucher.AdjustedQuantity * -1;
                v.voucher = voucher;
                v.price = adjPrice;

                tempAdjVoucherList.Add(v);
            }
            adjVoucherViewModel.adjustmentVoucherLists = tempAdjVoucherList;
            return View(adjVoucherViewModel);
        }

        public ActionResult ChangeAdjVoucherStatus(string voucherId)
        {
            int vId = int.Parse(voucherId);
            User user = (User)Session["usersession"];
            itemServices.ChangeAdjVoucherStatus(vId, AdjustmentVoucherStatus.ISSUED, user.UserId);
            return RedirectToAction("RequestedVoucher", "StoreClerk");
        }

        public ActionResult VoucherHistory()
        {

            List<AdjustmentVoucherList> tempAdjVoucherList = new List<AdjustmentVoucherList>();
            AdjustmentVoucherViewModel adjVoucherViewModel = new AdjustmentVoucherViewModel();
            List<AdjustmentVoucher> adjustmentVouchers = itemServices.GetAdjVouchers(AdjustmentVoucherStatus.ISSUED);

            foreach (AdjustmentVoucher voucher in adjustmentVouchers)
            {
                var v = new AdjustmentVoucherList();
                double price = itemServices.GetPriceByItemCode(voucher.Item.ItemCode);
                double adjPrice = price * voucher.AdjustedQuantity;
                v.voucher = voucher;
                v.price = adjPrice;

                tempAdjVoucherList.Add(v);
            }
            adjVoucherViewModel.adjustmentVoucherLists = tempAdjVoucherList;
            return View(adjVoucherViewModel);
        }

        [HttpGet]
        public ActionResult VoucherDetail(int Id)
        {
            AdjustmentVoucher adjustmentVoucher = itemServices.GetAdjVoucherById(Id);
            return PartialView("VoucherDetail", adjustmentVoucher);
        }

        public ActionResult EditItem(string iCode)
        {
            Item item = itemServices.GetItem(iCode);
            return View("EditItem", item);
        }

        [HttpPost]
        public ActionResult UpdateItem(Item item)
        {
            if (ModelState.IsValid)
            {
                itemServices.UpdateItem(item);
                return RedirectToAction("QuantityRecommendation");
            }
            return View("EditItem", item);
        }

        public ActionResult GenerateReport()
        {
            var deplistall = itemServices.GetAllDisbursementByDep();

            var deplist = itemServices.GetAllDisbursementByDate(DateTime.Today, DateTime.Now);
            List<ReportItem> reportItems = new List<ReportItem>();
            foreach (DepDisbursementList d in deplist)
            {
                foreach (var item in d.DisburseItems)
                {
                    ReportItem reportItem = null;
                    if (reportItems.Count != 0)
                    {
                        reportItem = reportItems.Find(m => m.Item.ItemCode == item.RequestItem.ItemCode);
                    }
                    if (reportItem != null)
                    {
                        reportItem.DisbursedQty += item.DisbursedQuantity;
                        reportItem.RequestedQty += item.RequestedQuantity;
                    }
                    else
                    {
                        reportItem = new ReportItem
                        {
                            Item = item.RequestItem,
                            DisbursedQty = item.DisbursedQuantity,
                            RequestedQty = item.RequestedQuantity
                        };
                        reportItems.Add(reportItem);
                    }
                }
            }

            Department department = new Department
            {
                DepartmentName = "All",
                DepartmentCode = "all"
            };

            GenerateReportViewModel generateReportViewModel = new GenerateReportViewModel
            {
                ToDate = DateTime.Now,
                Departments = deplistall.Select(m => m.Department).Distinct().ToList()
            };
            generateReportViewModel.Departments.Add(department);


            generateReportViewModel.Departments = generateReportViewModel.Departments.OrderBy(m => m.DepartmentName).ToList();
            generateReportViewModel.ReportItems = reportItems;

            generateReportViewModel.ToDate = DateTime.Today;
            generateReportViewModel.FromDate = DateTime.Today;

            return View("generateReportTest2", generateReportViewModel);

        }

        [HttpPost]
        public ActionResult GenerateReport1(GenerateReportViewModel generateReportViewModel)
        {
            DateTime fromDate = generateReportViewModel.FromDate;
            if (fromDate == DateTime.Today)
            {
                fromDate = DateTime.Today;
            }
            DateTime toDate = generateReportViewModel.ToDate;
            if (toDate == DateTime.Today)
            {
                toDate = DateTime.Now;
            }
            String depCode = generateReportViewModel.DepCode;
            var deplistall = itemServices.GetAllDisbursementByDep();
            var deplist = new List<DepDisbursementList>();

            if (depCode != "all")
            {
                deplist = itemServices.GetAllDisbursementByDateAndDepCode(fromDate, toDate, depCode);
            }
            else
            {
                deplist = itemServices.GetAllDisbursementByDate(fromDate, toDate);
            }

            List<ReportItem> reportItems = new List<ReportItem>();
            foreach (DepDisbursementList d in deplist)
            {
                foreach (var item in d.DisburseItems)
                {
                    ReportItem reportItem = null;
                    if (reportItems.Count != 0)
                    {
                        reportItem = reportItems.Find(m => m.Item.ItemCode == item.RequestItem.ItemCode);
                    }
                    if (reportItem != null)
                    {
                        reportItem.DisbursedQty += item.DisbursedQuantity;
                        reportItem.RequestedQty += item.RequestedQuantity;
                    }
                    else
                    {
                        reportItem = new ReportItem
                        {
                            Item = item.RequestItem,
                            DisbursedQty = item.DisbursedQuantity,
                            RequestedQty = item.RequestedQuantity
                        };
                        reportItems.Add(reportItem);
                    }
                }
            }

            Department department = new Department
            {
                DepartmentName = "All",
                DepartmentCode = "all"
            };

            GenerateReportViewModel generateReportViewModel2 = new GenerateReportViewModel
            {
                ToDate = toDate,
                Departments = deplistall.Select(m => m.Department).Distinct().ToList()
            };
            generateReportViewModel2.Departments.Add(department);


            generateReportViewModel2.Departments = generateReportViewModel2.Departments.OrderBy(m => m.DepartmentName).ToList();
            generateReportViewModel2.ReportItems = reportItems;

            return View("generateReportTest2", generateReportViewModel2);
        }

        public async Task<ActionResult> QuantityRecommendation()
        {

            var quantityRecommendationViewModelList = new List<QuantityRecommendationViewModel>();

            List<MLModel> mlList = new List<MLModel>();
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var url = "http://127.0.0.1:5000/recommend_quantity";
                    int dt = DateTime.Now.Day;
                    int currentmth = DateTime.Now.Month;
                    int currentyear = DateTime.Now.Year;
                    int mth;
                    int yr;
                    if (dt > 15)
                    {
                        mth = currentmth * 2;
                    }
                    else
                    {
                        mth = (currentmth * 2) - 1;
                    }
                    if (mth > 24)
                    {
                        yr = currentyear + 1;
                        mth = 1;
                    }
                    yr = currentyear;
                    var itemList = itemServices.GetItems();
                    foreach (Item item in itemList)
                    {
                        var mlm = new MLModel
                        {
                            item_number = item.ItemCode,
                            month = mth,
                            year = yr
                        };
                        mlList.Add(mlm);
                    }


                    RecommendList recommendList = new RecommendList
                    {
                        recommend_req = new List<MLModel>()
                    };
                    recommendList.recommend_req.AddRange(mlList);
                    HttpResponseMessage result;
                    bool isConnnFail = false;
                    try
                    {
                        result = await client.PostAsync(url, recommendList.AsJson());
                        if (result.IsSuccessStatusCode)
                        {

                            String res = result.Content.ReadAsStringAsync().Result;
                            var RecommendRes = JsonConvert.DeserializeObject<RecommendRes>(res);
                            //return Content(result.Content.ReadAsStringAsync().Result);

                            foreach (var recres in RecommendRes.recommend_res)
                            {
                                QuantityRecommendationViewModel quantityRecommendationViewModel = new QuantityRecommendationViewModel();

                                Item item = itemServices.GetItem(recres.ItemNumber);
                                quantityRecommendationViewModel.Item = item;
                                quantityRecommendationViewModel.month = recres.Month;
                                quantityRecommendationViewModel.recommend_Qty = recres.Quantity;
                                quantityRecommendationViewModel.year = recres.Year;
                                quantityRecommendationViewModelList.Add(quantityRecommendationViewModel);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        isConnnFail = true;
                    }
                    if (isConnnFail)
                    {
                        foreach (var it in itemList)
                        {
                            QuantityRecommendationViewModel quantityRecommendationViewModel = new QuantityRecommendationViewModel
                            {
                                Item = it,
                                recommend_Qty = -1
                            };
                            quantityRecommendationViewModelList.Add(quantityRecommendationViewModel);
                        }
                    }
                }
            }

            return View("ViewCatalogue", quantityRecommendationViewModelList);
        }

        public async Task<ActionResult> GeneratePrediction(GeneratePredictionViewModel generatePredictionViewModelPost)
        {

            GeneratePredictionViewModel generatePredictionViewModel = new GeneratePredictionViewModel();
            List<MLModel> mlList = new List<MLModel>();

            generatePredictionViewModel.itemcode = "C001";
            if (generatePredictionViewModelPost.Item != null)
            {
                generatePredictionViewModel.itemcode = generatePredictionViewModelPost.Item.ItemCode;
            }


            DateTime toDate = generatePredictionViewModelPost.ToDate;
            DateTime fromDate = generatePredictionViewModelPost.FromDate;



            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var url = "http://127.0.0.1:5000/recommend_quantity";
                    int dt = DateTime.Now.Day;
                    int currentmth = DateTime.Now.Month;
                    int currentyear = DateTime.Now.Year;
                    int yr;
                    int firstmth = -1;
                    int firstyear = -1;
                    int count = 0;
                    int endmth = -1;
                    int endyear = -1;
                    int firstday = fromDate.Day;
                    int endday = toDate.Day;



                    if (toDate == fromDate)
                    {
                        int startpoint = 2;

                        if (dt > 15)
                        {
                            currentmth -= startpoint;
                            if (currentmth == 0)
                            {
                                firstmth = 12;
                                firstyear = currentyear - 1;
                                firstmth *= 2;
                            }
                            else if (currentmth == -1)
                            {
                                firstmth = 11;
                                firstyear = currentyear - 1;
                                firstmth *= 2;
                            }
                            else
                            {
                                firstmth *= 2;
                            }
                        }
                        else
                        {
                            currentmth -= startpoint;
                            if (currentmth == 0)
                            {
                                firstmth = 12;
                                firstyear = currentyear - 1;
                                firstmth = firstmth * 2 - 1;
                            }
                            else if (currentmth == -1)
                            {
                                firstmth = 11;
                                firstyear = currentyear - 1;
                                firstmth = firstmth * 2 - 1;
                            }
                            else
                            {
                                firstmth = firstmth * 2 - 1;
                            }

                        }

                        yr = firstyear;
                        count = 8;


                        var itemSelected = itemServices.GetItem(generatePredictionViewModel.itemcode);

                        for (int i = count; i > 0; i--)
                        {
                            if (itemSelected != null)
                            {
                                var mlm = new MLModel
                                {
                                    item_number = itemSelected.ItemCode,
                                    month = firstmth,
                                    year = yr
                                };
                                mlList.Add(mlm);
                                firstmth += 1;
                                if (firstmth >= 25 && yr != currentyear)
                                {
                                    yr += 1;
                                    firstmth = 1;
                                }
                                if (firstmth >= 25)
                                {
                                    firstmth = 1;
                                }
                            }
                        }
                    }
                    else
                    {


                        firstmth = fromDate.Month;
                        firstyear = fromDate.Year;
                        endmth = toDate.Month;
                        endyear = toDate.Year;

                        if (firstday > 15)
                        {
                            firstmth *= 2;
                        }
                        else
                        {
                            firstmth = firstmth * 2 - 1;
                        }
                        if (endday > 15)
                        {
                            endmth *= 2;
                        }
                        else
                        {
                            endmth = endmth * 2 - 1;
                        }

                        yr = firstyear;
                        if (firstyear == endyear)
                        {
                            count = endmth - firstmth + 5;
                        }
                        else
                        {
                            int difyear = endyear - firstyear;
                            for (int i = difyear; i > 0; i--)
                            {
                                difyear *= 24;
                                endmth += difyear;
                                count = endmth - firstmth + 5;
                            }
                        }



                        var itemSelected = itemServices.GetItem(generatePredictionViewModel.itemcode);

                        for (int i = count; i > 0; i--)
                        {
                            if (itemSelected != null)
                            {
                                var mlm = new MLModel
                                {
                                    item_number = itemSelected.ItemCode,
                                    month = firstmth,
                                    year = yr
                                };
                                mlList.Add(mlm);
                                firstmth += 1;
                                if (firstmth >= 25 && yr != currentyear)
                                {
                                    yr += 1;
                                    firstmth = 1;
                                }
                                if (firstmth >= 25)
                                {
                                    firstmth = 1;
                                }
                            }
                        }
                    }
                    RecommendList recommendList = new RecommendList
                    {
                        recommend_req = new List<MLModel>()
                    };
                    recommendList.recommend_req.AddRange(mlList);

                    var quantityRecommendationViewModelList = new List<QuantityRecommendationViewModel>();
                    try
                    {

                        HttpResponseMessage result = await client.PostAsync(url, recommendList.AsJson());
                        if (result.IsSuccessStatusCode)
                        {

                            String res = result.Content.ReadAsStringAsync().Result;
                            var RecommendRes = JsonConvert.DeserializeObject<RecommendRes>(res);
                            //return Content(result.Content.ReadAsStringAsync().Result);



                            foreach (var recres in RecommendRes.recommend_res)
                            {
                                Item item = itemServices.GetItem(recres.ItemNumber);
                                generatePredictionViewModel.Item = item;

                                generatePredictionViewModel.Items = itemServices.GetItems();
                                var quantityRecommendationViewModel = new QuantityRecommendationViewModel
                                {
                                    month = recres.Month,
                                    recommend_Qty = recres.Quantity,
                                    year = recres.Year
                                };
                                double mthdivide = ((double)recres.Month / 2);
                                if (mthdivide == 0)
                                {
                                    mthdivide = 1;
                                }

                                quantityRecommendationViewModel.strMonth =
                                    DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(
                                        (int)Math.Ceiling(mthdivide));
                                quantityRecommendationViewModelList.Add(quantityRecommendationViewModel);
                                generatePredictionViewModel.quantityRecommendationViewModels =
                                    quantityRecommendationViewModelList;
                            }
                        }
                    }
                    catch
                    {
                        return View("pythonError");
                    }

                }
            }
            generatePredictionViewModel.FromDate = DateTime.Today;
            generatePredictionViewModel.ToDate = DateTime.Today;
            return View(generatePredictionViewModel);
        }

        public ActionResult GenerateCategory(GenerateCategoryViewModel generateCategoryViewModelPost)
        {

            var categoryList = itemServices.GetCategoryLists();
            GenerateCategoryViewModel generateCategoryViewModel = new GenerateCategoryViewModel
            {
                categorylist = categoryList
            };

            DateTime fromDate = generateCategoryViewModelPost.FromDate;
            DateTime toDate = generateCategoryViewModelPost.ToDate;

            var DislistByItemCode = itemServices.GetDisburseItemsByItemCode(generateCategoryViewModelPost.catCode, fromDate, toDate);

            List<ReportItem> reportItems = new List<ReportItem>();

            foreach (var disburseItem in DislistByItemCode)
            {
                ReportItem reportItem = null;

                if (reportItems.Count != 0)
                {

                    reportItem = reportItems.Find(m => m.Item.ItemCode == disburseItem.RequestItem.ItemCode);

                }

                if (reportItem != null)
                {
                    reportItem.DisbursedQty += disburseItem.DisbursedQuantity;
                    reportItem.RequestedQty += disburseItem.RequestedQuantity;
                }
                else
                {
                    reportItem = new ReportItem
                    {
                        Item = disburseItem.RequestItem,
                        DisbursedQty = disburseItem.DisbursedQuantity,
                        RequestedQty = disburseItem.RequestedQuantity
                    };
                    reportItems.Add(reportItem);
                }

            }
            generateCategoryViewModel.ReportItems = reportItems;

            generateCategoryViewModel.ToDate = DateTime.Today;
            generateCategoryViewModel.FromDate = DateTime.Today;

            return View(generateCategoryViewModel);
        }

        public ActionResult TenderQuotationForm()
        {
            ItemSupplierListViewModel supplierListViewModel = new ItemSupplierListViewModel();
            List<Supplier> Suppliers = itemSupplierServices.GetAllSuppliers();
            supplierListViewModel.Suppliers = Suppliers;
            return View(supplierListViewModel);
        }

        [HttpPost]
        public ActionResult GetItemListBySupplier(ItemSupplierListViewModel supplierListViewModel)
        {
            var code = supplierListViewModel.SelectedSupplierCode;

            List<Supplier> Suppliers = itemSupplierServices.GetAllSuppliers();
            List<ItemSupplier> itemSuppliers = itemSupplierServices.GetItemSuppliersBySupplierCode(code);

            ItemSupplierListViewModel itemSupplierListViewModel = new ItemSupplierListViewModel
            {
                Suppliers = Suppliers,
                itemSuppliers = itemSuppliers,
                SelectedSupplierCode = code
            };

            return View("TenderQuotationForm", itemSupplierListViewModel);
        }

        public ActionResult ReorderReport()
        {
            ReorderReportViewModel reorderReport = new ReorderReportViewModel
            {
                PurchaseOrderLists = new List<PurchaseOrder>(),
                FromDate = DateTime.Now,
                ToDate = DateTime.Today
            };
            return View(reorderReport);
        }

        [HttpPost]
        public ActionResult GetPurchaseOrderList(ReorderReportViewModel reorderReport)
        {
            var status = reorderReport.SelectedStatus;
            List<PurchaseOrder> purchasorderList = new List<PurchaseOrder>();

            if (reorderReport.ToDate == DateTime.Today)
            {
                reorderReport.ToDate = DateTime.Now;
            }

            if (status != 2)
            {
                purchasorderList = dbContext.PurchaseOrders.Include(m => m.PurchaseItems.Select(a => a.Item)).Where(m => m.PurchaseOrderStatus == (Enums.PurchaseOrderStatus)status).Where(m => m.PurchaseOrderDate <= reorderReport.ToDate && m.PurchaseOrderDate >= reorderReport.FromDate).ToList();
            }
            else
            {
                purchasorderList = dbContext.PurchaseOrders.Include(m => m.PurchaseItems.Select(a => a.Item)).Where(m => m.PurchaseOrderDate >= reorderReport.FromDate && m.PurchaseOrderDate <= reorderReport.ToDate).ToList();
            }

            ReorderReportViewModel reorderReportViewModel = new ReorderReportViewModel
            {
                FromDate = reorderReport.FromDate,
                ToDate = reorderReport.ToDate,
                PurchaseOrderLists = purchasorderList,
                SelectedStatus = reorderReport.SelectedStatus
            };

            return View("ReorderReport", reorderReportViewModel);
        }

    }
}
