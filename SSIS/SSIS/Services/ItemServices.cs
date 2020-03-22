using SSIS.Enum;
using SSIS.Enums;
using SSIS.Models;
using SSIS.Models.Util_Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace SSIS.Services
{
    public class ItemServices
    {
        private SSISDbContext dbContext;

        public ItemServices(SSISDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Item GetItem(string itemCode)
        {
            return dbContext.Items.SingleOrDefault(m => m.ItemCode == itemCode);
        }

        public Item GetItembyDes(string itemDes)
        {
            return dbContext.Items.SingleOrDefault(m => m.Description == itemDes);
        }

        public List<Item> GetItems()
        {
            return dbContext.Items.ToList();
        }

        public List<Item> GetActiveItems()
        {
            return dbContext.Items.Where(m => m.IsActive == true).ToList();
        }


        public RequestItem SaveRequestItem(RequestItem requestItem)
        {
            var requestItemDb = dbContext.RequestItems.Add(requestItem);
            dbContext.SaveChanges();
            return requestItemDb;
        }


        public RequisationForm SaveRequisationForm(RequisationForm requisationForm)
        {
            RequisationForm requisationFormDb;
            if (requisationForm.Id == 0)
            {
                requisationFormDb = dbContext.RequisationForms.Add(requisationForm);
                dbContext.SaveChanges();
                requisationFormDb.RequestNumber = "R" + requisationFormDb.Id.ToString("D5");
            }
            else
            {
                requisationFormDb = dbContext.RequisationForms.SingleOrDefault(m => m.Id == requisationForm.Id);
                requisationFormDb.RequestItems = requisationForm.RequestItems;
                requisationFormDb.RequestedDate = requisationForm.RequestedDate;
            }

            dbContext.SaveChanges();

            return requisationFormDb;
        }


        public List<RequisationForm> GetAdditionStatusRequisationForms()
        {
            return dbContext.RequisationForms.Include(m => m.RequestedBy).Include(m => m.RequestedBy.Department)
                .Include(m => m.RequestItems.Select(a => a.Item))
                .Include(m => m.RequestedBy.Department.DepartmentRepresentative)
                .Where(m => m.FormStatus == Enums.FormStatus.PENDING_ADDITION).ToList();
        }

        public List<RequisationForm> GetAllPendingRequisationFormsbyEmpID(int empID)
        {
            return dbContext.RequisationForms.Include(m => m.RequestedBy).Include(m => m.RequestedBy.Department)
                .Where(m => m.RequestedBy.UserId == empID && m.FormStatus == FormStatus.PENDING_APPROVAL).ToList();
        }



        public List<RequisationForm> GetAllPendingRequisationFormsbyDep(string depCode)
        {
            return dbContext.RequisationForms.Include(m => m.RequestedBy).Include(m => m.RequestedBy.Department)
                .Where(m => m.RequestedBy.DepartmentCode == depCode && m.FormStatus == FormStatus.PENDING_APPROVAL)
                .OrderByDescending(r => r.RequestedDate).ToList();
        }

        public List<RequisationForm> GetAllPendingRequisationFormsbyDepAPI(string depCode)
        {
            return dbContext.RequisationForms.Include(m => m.RequestedBy)
                .Include(m => m.RequestItems.Select(a => a.Item))
                .Where(m => m.RequestedBy.DepartmentCode == depCode && m.FormStatus == FormStatus.PENDING_APPROVAL)
                .OrderByDescending(r => r.RequestedDate).ToList();
        }

        public RequisationForm GetRequisationFormbyId(int id)
        {
            return dbContext.RequisationForms.Include(m => m.RequestedBy)
                .Include(i => i.RequestItems.Select(it => it.Item))
                .SingleOrDefault(m => m.Id == id);
        }


        public void DeleteRequstionForm(int id)
        {
            var reqForm = GetRequisationFormbyId(id);
            dbContext.RequisationForms.Remove(reqForm);
            dbContext.SaveChanges();
        }

        public void ChangeStatusWithComments(int UserId, int id, FormStatus formStatus, string comments)
        {
            var reqForm = GetRequisationFormbyId(id);
            reqForm.HandeledBy = dbContext.Employees.SingleOrDefault(m => m.UserId == UserId);
            reqForm.HandeledDate = DateTime.Now;
            reqForm.FormStatus = formStatus;
            reqForm.Comments = comments;
            dbContext.SaveChanges();
        }

        public List<RequisationForm> GetAllOtherRequisationFormsbyEmpID(int userId)
        {
            return dbContext.RequisationForms.Include(m => m.HandeledBy)
                .Where(m => m.RequestedBy.UserId == userId && m.FormStatus != FormStatus.PENDING_APPROVAL).ToList();

        }

        public List<RequisationForm> GetAllOtherPendingRequisationFormsbyDep(string departmentCode)
        {
            return dbContext.RequisationForms.Include(m => m.HandeledBy).Include(m => m.RequestedBy.Department)
                .Where(m => m.RequestedBy.DepartmentCode == departmentCode &&
                            m.FormStatus != FormStatus.PENDING_APPROVAL).OrderByDescending(r => r.RequestedDate)
                .ToList();

        }

        public List<RequisationForm> GetAllPendingAdditionRequisationFormsbyDep(string departmentCode)
        {
            return dbContext.RequisationForms.Include(m => m.RequestedBy.Department)
                .Include(m => m.RequestItems.Select(a => a.Item)).Where(m =>
                    m.RequestedBy.DepartmentCode == departmentCode && m.FormStatus == FormStatus.PENDING_ADDITION)
                .ToList();
        }

        public void ChangeStatusToRetrivalByDep(string departmentCode)
        {
            var lists = dbContext.RequisationForms.Include(m => m.RequestedBy.Department)
                .Include(m => m.RequestItems.Select(a => a.Item)).Where(m =>
                    m.RequestedBy.DepartmentCode == departmentCode && m.FormStatus == FormStatus.PENDING_ADDITION)
                .ToList();
            foreach (var rf in lists)
            {
                rf.FormStatus = FormStatus.PENDING_RETRIVAL;
            }

            dbContext.SaveChanges();
        }

        public List<RequisationForm> GetFormByStatusRetrievel()
        {
            return dbContext.RequisationForms.Where(r => r.FormStatus == Enums.FormStatus.PENDING_RETRIVAL)
                .Include(i => i.RequestItems.Select(it => it.Item)).Include(m => m.RequestedBy)
                .Include(m => m.RequestedBy.Department).ToList();
        }

        public void SaveDisbursedItem(DisburseItem disburse)
        {
            dbContext.DisburseItems.Add(disburse);
            dbContext.SaveChanges();
        }

        public void SaveDepDisbursedList(DepDisbursementList depDisbursement)
        {
            var depDisbursementDb = dbContext.DisbursementLists.Add(depDisbursement);
            dbContext.SaveChanges();
            depDisbursementDb.DepDisbursementListNumber = "D" + depDisbursementDb.DepDisbursementListId.ToString("D5");
            dbContext.SaveChanges();
        }

        public void ChangeStatusToDisbursement(int id, FormStatus formStatus)
        {
            var form = dbContext.RequisationForms.SingleOrDefault(m => m.Id == id);
            form.FormStatus = formStatus;
            dbContext.SaveChanges();
        }

        public List<StockCard> GetStockCards(string itemCode)
        {
            return dbContext.StockCards.Include(m => m.Item).Where(m => m.Item.ItemCode == itemCode).ToList();
        }

        public void SaveVoucher(Voucher voucher)
        {
            voucher = dbContext.Vouchers.Add(voucher);
            var item = dbContext.Items.SingleOrDefault(m => m.ItemCode == voucher.Item.ItemCode);
            item.CurrentQuantity += voucher.Quantity;
            StockCard stockCard = new StockCard
            {
                Item = item,
                Comments = voucher.Comment,
                Balance = item.CurrentQuantity,
                ChangeInQuantity = voucher.Quantity,
                TransactionDate = voucher.CreatedDate
            };
            dbContext.StockCards.Add(stockCard);
            dbContext.SaveChanges();
        }

        public List<DepDisbursementList> GetDepDisbursementLists()
        {
            List<DepDisbursementList> returnList = new List<DepDisbursementList>();
            returnList = dbContext.DisbursementLists.Include(s => s.DisburseItems.Select(m => m.DepDisbursementList))
                .Include(c => c.Department).Include(di => di.DisburseItems.Select(dis => dis.RequestItem))
                .Include(r => r.Department.DepartmentRepresentative).Include(c => c.Department.CollectionPoint)
                .Where(w => w.DisbursementStatus != DisbursementStatus.CANCELLED &&
                            w.DisbursementStatus != DisbursementStatus.COLLECTED).Where(m => m.DisburseItems.Count > 0)
                .ToList();
            return returnList;
        }

        public int RemoveDisbursementItems(int Id, int DepId)
        {
            DisburseItem disburseItem = dbContext.DisburseItems.Include(ri => ri.RequestItem)
                .SingleOrDefault(s => s.DisburseItemID == Id);
            if (disburseItem != null)
            {
                DepDisbursementList updateDepDisbursementList = dbContext.DisbursementLists
                    .Include(i => i.DisburseItems).SingleOrDefault(si => si.DepDisbursementListId == DepId);
                if (updateDepDisbursementList.DisburseItems.Contains(disburseItem))
                {
                    updateDepDisbursementList.DisburseItems.Remove(disburseItem);
                    dbContext.DisburseItems.Remove(disburseItem);
                }
            }

            return dbContext.SaveChanges();
        }

        public int CancelDisbursementList(int id)
        {
            DepDisbursementList updateDepDisbursementList =
                dbContext.DisbursementLists.SingleOrDefault(s => s.DepDisbursementListId == id);
            updateDepDisbursementList.DisburseDate = DateTime.Today;

            updateDepDisbursementList.DisbursementStatus = DisbursementStatus.CANCELLED;

            return dbContext.SaveChanges();
        }

        public String GenerateOTP()
        {
            Random rand = new Random();
            String strNumber = (rand.Next(100000, 999999)).ToString();
            return strNumber;
        }

        public int AnknowledgeDisbursementItems(DepDisbursementList paraDepDisbursement)
        {
            DepDisbursementList updateDepDisbursement =
                GetDepDisbursementById(paraDepDisbursement.DepDisbursementListId);

            foreach (DisburseItem temp in paraDepDisbursement.DisburseItems)
            {
                StockCard stockCard = new StockCard();
                Item item = dbContext.Items.Include(s => s.StockCards).Include(su => su.ItemSuppliers)
                    .SingleOrDefault(i => i.ItemCode == temp.RequestItem.ItemCode);
                DisburseItem disburseItem =
                    updateDepDisbursement.DisburseItems.SingleOrDefault(s =>
                        s.RequestItem.ItemCode == temp.RequestItem.ItemCode);
                if (disburseItem != null)
                {
                    disburseItem.DisbursedQuantity = temp.DisbursedQuantity;
                    stockCard.ChangeInQuantity = temp.DisbursedQuantity * -1;
                    stockCard.TransactionDate = DateTime.Now;
                    stockCard.Comments = updateDepDisbursement.Department.DepartmentName;
                    item.CurrentQuantity -= temp.DisbursedQuantity;
                    stockCard.Balance = item.CurrentQuantity;
                    item.StockCards.Add(stockCard);
                    dbContext.SaveChanges();
                }


            }

            updateDepDisbursement.OTP = GenerateOTP();
            updateDepDisbursement.DisbursementStatus = DisbursementStatus.WAITING_FOR_OTP;
            return dbContext.SaveChanges();
        }

        public DepDisbursementList SaveResentOTP(int id)
        {
            DepDisbursementList depDisbursementList = GetDepDisbursementById(id);
            string newOTP = GenerateOTP();
            depDisbursementList.OTP = newOTP;
            dbContext.SaveChanges();
            return depDisbursementList;
        }

        public DepDisbursementList GetDepDisbursementById(int id)
        {
            DepDisbursementList depDisbursementList = dbContext.DisbursementLists.Include(s => s.DisburseItems)
                .Include(di => di.DisburseItems.Select(dis => dis.RequestItem))
                .Include(c => c.Department).Include(r => r.Department.DepartmentHead)
                .Include(r => r.Department.DepartmentRepresentative).Include(c => c.Department.CollectionPoint)
                .SingleOrDefault(s => s.DepDisbursementListId == id);

            return depDisbursementList;
        }

        public int GenerateRequisationForm(int DisId)
        {
            DepDisbursementList depDisbursementList =
                dbContext.DisbursementLists.SingleOrDefault(s => s.DepDisbursementListId == DisId);

            depDisbursementList.isGenerated = true;
            return dbContext.SaveChanges();
        }

        public List<DepDisbursementList> GetDepDisbursementHistory()
        {

            List<DepDisbursementList> returnList = new List<DepDisbursementList>();
            returnList = dbContext.DisbursementLists.Include(s => s.DisburseItems).Include(c => c.Department)
                .Include(di => di.DisburseItems.Select(dis => dis.RequestItem))
                .Include(r => r.Department.DepartmentRepresentative).Include(c => c.Department.CollectionPoint).Where(
                    w => w.DisbursementStatus == SSIS.Enums.DisbursementStatus.CANCELLED ||
                         w.DisbursementStatus == DisbursementStatus.COLLECTED).Where(m => m.DisburseItems.Count > 0)
                .ToList();

            return returnList;

        }



        public int ChangeDisbursementForOTP(int id)
        {
            DepDisbursementList depDisbursement =
                dbContext.DisbursementLists.SingleOrDefault(s => s.DepDisbursementListId == id);
            depDisbursement.DisbursementStatus = SSIS.Enums.DisbursementStatus.COLLECTED;
            depDisbursement.DisburseDate = DateTime.Today;
            return dbContext.SaveChanges();
        }


        public PurchaseOrder SavePurchaseOrder(PurchaseOrder pOrder)
        {
            pOrder = dbContext.PurchaseOrders.Add(pOrder);
            dbContext.SaveChanges();
            pOrder.PurchaseOrderNumber = "PO" + pOrder.Id.ToString("D5");
            dbContext.SaveChanges();
            return pOrder;
        }

        public PurchaseItem SavePurchaseItem(PurchaseItem pItem)
        {
            PurchaseItem item = dbContext.PurchaseItems.Add(pItem);
            dbContext.SaveChanges();
            return item;
        }


        public PurchaseOrder GetPurchaseOrder(int pId)
        {
            return dbContext.PurchaseOrders
                .Include(i => i.Supplier)
                .Include(i => i.PurchaseItems.Select(m => m.Item))
                .SingleOrDefault(p => p.Id == pId);

        }

        public void UpdatePurchaseOrderStatus(int pId)
        {
            PurchaseOrder p = GetPurchaseOrder(pId);
            p.PurchaseOrderStatus = PurchaseOrderStatus.DELIVERED;
            p.DeliveredDate = DateTime.Today;
            dbContext.SaveChanges();

        }

        public List<PurchaseOrder> GetDelieveredPurchaseOrder()
        {
            return dbContext.PurchaseOrders
                .Include(i => i.Supplier)
                .Include(i => i.PurchaseItems.Select(m => m.Item))
                .Where(p => p.PurchaseOrderStatus == Enums.PurchaseOrderStatus.DELIVERED).ToList();
        }

        public List<PurchaseOrder> GetPendingPurchaseOrder()
        {
            return dbContext.PurchaseOrders
                .Include(i => i.Supplier)
                .Include(i => i.PurchaseItems.Select(m => m.Item))
                .Where(p => p.PurchaseOrderStatus == Enums.PurchaseOrderStatus.PENDING).ToList();
        }

        public StockCard AddStockCard(int pId)
        {
            StockCard s = new StockCard();
            PurchaseOrder po = GetPurchaseOrder(pId);
            for (int i = 0; i < po.PurchaseItems.Count; i++)
            {

                s.ChangeInQuantity = po.PurchaseItems[i].Quantity;
                s.TransactionDate = DateTime.Now;
                s.Comments = po.Supplier.SupplierName;
                s.Item = GetItem(po.PurchaseItems[i].Item.ItemCode);
                s.Balance = s.Item.CurrentQuantity;
                s = dbContext.StockCards.Add(s);
                dbContext.SaveChanges();
            }

            return s;
        }

        public Item UpdateItemQty(int pId)
        {
            Item item = new Item();

            PurchaseOrder po = GetPurchaseOrder(pId);


            for (int i = 0; i < po.PurchaseItems.Count; i++)
            {
                item = po.PurchaseItems[i].Item;
                if (item.ItemCode == po.PurchaseItems[i].Item.ItemCode)
                {
                    item.CurrentQuantity += po.PurchaseItems[i].Quantity;
                }

                dbContext.SaveChanges();
            }

            return item;
        }

        public List<Voucher> GetVouchers()
        {
            return dbContext.Vouchers.Include(m => m.Item).Where(v => v.VoucherStatus == VoucherStatus.ACTIVE).ToList();
        }

        public void SaveAdjVoucher(AdjustmentVoucher adjustmentVoucher)
        {
            var adjVoucher = dbContext.AdjustmentVouchers.Add(adjustmentVoucher);
            dbContext.SaveChanges();
            adjVoucher.VoucherNumber = "R" + adjVoucher.VoucherId.ToString("D5");
            dbContext.SaveChanges();
        }

        public void ChangeVoucherStatus(int id, Enums.VoucherStatus Status)
        {
            var voucher = dbContext.Vouchers.SingleOrDefault(m => m.Id == id);
            voucher.VoucherStatus = Status;
            dbContext.SaveChanges();
        }

        public List<AdjustmentVoucher> GetAdjVouchers(AdjustmentVoucherStatus status)
        {
            return dbContext.AdjustmentVouchers.Include(i => i.Item).Include(a => a.IssuedBy)
                .Include(i => i.RequestedBy).Include(i => i.Vouchers).Where(v => v.VoucherStatus == status).ToList();
        }

        public double GetPriceByItemCode(string itemCode)
        {
            ItemSupplier s = dbContext.ItemSuppliers.Where(i => i.Item.ItemCode == itemCode && i.Priority == 1)
                .SingleOrDefault();
            double price = s.Price;
            return price;
        }

        public void ChangeAdjVoucherStatus(int id, AdjustmentVoucherStatus Status, int userId)
        {
            var voucher = dbContext.AdjustmentVouchers.SingleOrDefault(m => m.VoucherId == id);
            voucher.VoucherStatus = Status;
            voucher.IssuedDate = System.DateTime.Now;
            voucher.IssuedBy = dbContext.StorePersons.SingleOrDefault(m => m.UserId == userId);
            dbContext.SaveChanges();
        }

        public AdjustmentVoucher GetAdjVoucherById(int vid)
        {
            return dbContext.AdjustmentVouchers.Include(i => i.Item).Include(i => i.RequestedBy)
                .Include(i => i.Vouchers).Where(v => v.VoucherId == vid).SingleOrDefault();
        }

        public Item UpdateItem(Item item)
        {
            var itemDb = GetItem(item.ItemCode);
            itemDb.Description = item.Description;
            itemDb.ReorderLevel = item.ReorderLevel;
            itemDb.ReorderQuantity = item.ReorderQuantity;
            itemDb.IsActive = item.IsActive;
            dbContext.SaveChanges();
            return itemDb;

        }

        public DepDisbursementList GetDepDisbursementListsByDepCode(string departmentCode)
        {

            List<DepDisbursementList> depList = dbContext.DisbursementLists
                .Where(i => i.Department.DepartmentCode == departmentCode).ToList();
            return depList.LastOrDefault();
        }

        public List<DepDisbursementList> GetDepDisbursementListByDepartment(Department department)
        {
            List<DepDisbursementList> returnList = new List<DepDisbursementList>();
            returnList = dbContext.DisbursementLists.Include(s => s.DisburseItems).Include(c => c.Department)
                .Include(di => di.DisburseItems.Select(dis => dis.RequestItem))
                .Include(r => r.Department.DepartmentRepresentative).Include(c => c.Department.CollectionPoint)
                .Where(w => (w.DisbursementStatus == DisbursementStatus.WAITING_FOR_ACK ||
                             w.DisbursementStatus == DisbursementStatus.WAITING_FOR_OTP) &&
                            w.Department.DepartmentCode == department.DepartmentCode && w.DisburseItems.Count > 0)
                .ToList();
            return returnList;

        }

        public List<DepDisbursementList> GetDepDisbursementHistoryByDepartment(Department department)
        {
            List<DepDisbursementList> returnList = new List<DepDisbursementList>();
            returnList = dbContext.DisbursementLists.Include(s => s.DisburseItems).Include(c => c.Department)
                .Include(di => di.DisburseItems.Select(dis => dis.RequestItem))
                .Include(r => r.Department.DepartmentRepresentative).Include(c => c.Department.CollectionPoint).Where(
                    w => (w.DisbursementStatus == DisbursementStatus.CANCELLED ||
                          w.DisbursementStatus == DisbursementStatus.COLLECTED) &&
                         w.Department.DepartmentCode == department.DepartmentCode && w.DisburseItems.Count > 0)
                .ToList();
            return returnList;
        }

        public void SaveReqFormfromAndroid(RequestForm requestForm)
        {
            var requisisationForm = dbContext.RequisationForms.SingleOrDefault(m => m.Id == requestForm.Id);
            requisisationForm.Comments = requestForm.Comments;
            requisisationForm.HandeledBy = dbContext.Employees.SingleOrDefault(m => m.UserId == requestForm.HandledBy);
            requisisationForm.HandeledDate = DateTime.Now;
            requisisationForm.FormStatus = requestForm.FormStatus;

            dbContext.SaveChanges();
        }


        public List<DepDisbursementList> GetAllDisbursementByDate(DateTime fromDate, DateTime toDate)
        {

            int fromMonth = fromDate.Month;
            int fromYear = fromDate.Year;
            int toMonth = toDate.Month;
            int toYear = toDate.Year;

            if (fromMonth == toMonth)
            {
                fromMonth = toMonth - 2;
                if (fromMonth == 0)
                {
                    fromMonth = 12;
                    fromYear -= 1;
                }

                if (fromMonth == -1)
                {
                    fromMonth = 11;
                    fromYear -= 1;
                }

                fromMonth.ToString("00");

                string a = "01";

                int from = int.Parse(fromYear.ToString() + fromMonth.ToString() + a);
                string from1 = from.ToString();

                fromDate = DateTime.ParseExact(from1, "yyyyMMdd", CultureInfo.InvariantCulture);
                return dbContext.DisbursementLists.Include(m => m.Department)
                    .Include(m => m.DisburseItems.Select(a1 => a1.RequestItem))
                    .Where(m => m.DisburseDate <= toDate && m.DisburseDate > fromDate)
                    .Where(m => m.DisburseItems.Count != 0)
                    .Where(m => m.DisbursementStatus == DisbursementStatus.COLLECTED).ToList();
            }

            else
            {
                return dbContext.DisbursementLists.Include(m => m.Department)
                    .Include(m => m.DisburseItems.Select(a1 => a1.RequestItem))
                    .Where(m => m.DisburseDate < toDate)
                    .Where(m => m.DisburseItems.Count != 0)
                    .Where(m => m.DisbursementStatus == DisbursementStatus.COLLECTED).ToList();
            }

        }

        public List<DepDisbursementList> GetAllDisbursementByDateAndDepCode(DateTime fromDate, DateTime toDate,
            string depcode)
        {

            int fromMonth = fromDate.Month;
            int fromYear = fromDate.Year;
            int toMonth = toDate.Month;
            int toYear = toDate.Year;

            if (fromMonth == toMonth)
            {
                fromMonth = toMonth - 2;
                if (fromMonth == 0)
                {
                    fromMonth = 12;
                    fromYear -= 1;
                }

                if (fromMonth == -1)
                {
                    fromMonth = 11;
                    fromYear -= 1;
                }

                fromMonth.ToString("00");

                string a = fromDate.Day.ToString("00");

                int from = int.Parse(fromYear.ToString() + fromMonth.ToString() + a);
                string from1 = from.ToString();

                fromDate = DateTime.ParseExact(from1, "yyyyMMdd", CultureInfo.InvariantCulture);
                return dbContext.DisbursementLists.Include(m => m.Department)
                    .Include(m => m.DisburseItems.Select(a1 => a1.RequestItem)).Where(m =>
                        m.DisburseDate <= toDate && m.DisburseDate >= fromDate &&
                        m.Department.DepartmentCode == depcode)
                    .Where(m => m.DisburseItems.Count != 0)
                    .Where(m => m.DisbursementStatus == DisbursementStatus.COLLECTED).ToList();
            }
            else
            {
                return dbContext.DisbursementLists.Include(m => m.Department)
                    .Include(m => m.DisburseItems.Select(a1 => a1.RequestItem)).Where(m =>
                        m.DisburseDate <= toDate && m.DisburseDate >= fromDate &&
                        m.Department.DepartmentCode == depcode).Where(m => m.DisburseItems.Count != 0).Where(m => m.DisbursementStatus == DisbursementStatus.COLLECTED).ToList();
            }
        }

        public List<DisburseItem> GetDisburseItemsByItemCode(string itemcode, DateTime fromDate, DateTime toDate)
        {
            return dbContext.DisburseItems.Include(m => m.RequestItem)
                .Include(m => m.DepDisbursementList).Where(m =>
                m.RequestItem.Category == itemcode && m.DepDisbursementList.DisburseDate <= toDate &&
                m.DepDisbursementList.DisburseDate >= fromDate).Where(m => m.DepDisbursementList.DisbursementStatus == DisbursementStatus.COLLECTED).ToList();
        }

        public List<String> GetCategoryLists()
        {
            return dbContext.Items.Select(m => m.Category).Distinct().ToList();
        }

        public DisburseItem GetDisburseItembyId(int id)
        {
            return dbContext.DisburseItems.Include(m => m.DepDisbursementList)
                .SingleOrDefault(m => m.DisburseItemID == id);
        }

        public List<PurchaseOrder> GetAllPurchaseOrders()
        {
            return dbContext.PurchaseOrders.Include(m => m.PurchaseItems.Select(a => a.Item)).ToList();
        }

        public List<PurchaseOrder> GetPurchaseOrdersBySelectedDateAndStatus(DateTime FromDate, DateTime ToDate,
            int status)
        {
            return dbContext.PurchaseOrders.Include(m => m.PurchaseItems.Select(a => a.Item))
                .Where(m => m.PurchaseOrderStatus == (Enums.PurchaseOrderStatus)status)
                .Where(m => m.PurchaseOrderDate <= ToDate && m.PurchaseOrderDate >= FromDate).ToList();
        }

        public List<PurchaseOrder> GetAllPurchaseOrdersBySelectedDate(DateTime FromDate, DateTime ToDate)
        {
            return dbContext.PurchaseOrders.Include(m => m.PurchaseItems.Select(a => a.Item))
                .Where(m => m.PurchaseOrderDate >= FromDate && m.PurchaseOrderDate <= ToDate).ToList();
        }

        public int AcknowledgefromAndroid(DisburseRequest disburseRequest)
        {
            DepDisbursementList updateDepDisbursement = GetDepDisbursementById(disburseRequest.Id);

            foreach (RequestItem temp in disburseRequest.RequestItems)
            {
                StockCard stockCard = new StockCard();
                Item item = dbContext.Items.Include(s => s.StockCards).Include(su => su.ItemSuppliers)
                    .SingleOrDefault(i => i.ItemCode == temp.Item.ItemCode);
                DisburseItem disburseItem =
                    updateDepDisbursement.DisburseItems.SingleOrDefault(s =>
                        s.RequestItem.ItemCode == temp.Item.ItemCode);
                if (disburseItem != null)
                {
                    disburseItem.DisbursedQuantity = temp.Quantity;
                    stockCard.ChangeInQuantity = temp.Quantity * -1;
                    stockCard.TransactionDate = DateTime.Now;
                    stockCard.Comments = updateDepDisbursement.Department.DepartmentName;
                    item.CurrentQuantity -= temp.Quantity;
                    stockCard.Balance = item.CurrentQuantity;
                    item.StockCards.Add(stockCard);
                    dbContext.SaveChanges();
                }
            }

            updateDepDisbursement.OTP = GenerateOTP();
            updateDepDisbursement.DisbursementStatus = DisbursementStatus.WAITING_FOR_OTP;
            return dbContext.SaveChanges();
        }

        public List<DepDisbursementList> GetAllDisbursementByDep()
        {
            return dbContext.DisbursementLists.Include(m => m.Department).Where(m => m.DisbursementStatus == DisbursementStatus.COLLECTED).Where(m => m.DisburseItems.Count != 0).ToList();
        }
    }
}
