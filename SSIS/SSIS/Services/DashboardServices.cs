using SSIS.Enums;
using SSIS.Models;
using System.Linq;

namespace SSIS.Services
{
    public class DashboardServices
    {
        private SSISDbContext dbContext;

        public DashboardServices(SSISDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public int GetLowStockItemsCount()
        {
            return dbContext.Items.Where(m => m.CurrentQuantity < m.ReorderLevel).Count();
        }
        public int GetPendingDisbursementList()
        {
            int c2 = dbContext.DisbursementLists.Where(m => m.DisbursementStatus == DisbursementStatus.WAITING_FOR_ACK && m.DisburseItems.Count > 0).Count();
            int c3 = dbContext.DisbursementLists.Where(m => m.DisbursementStatus == DisbursementStatus.WAITING_FOR_OTP && m.DisburseItems.Count > 0).Count();
            int count = c2 + c3;
            return count;
        }
        public int GetPendingPurOrderDelivery()
        {
            int count = dbContext.PurchaseOrders.Where(m => m.PurchaseOrderStatus == PurchaseOrderStatus.PENDING).Count();
            return count;
        }
        public int GetPendingRetrievalList()
        {
            return dbContext.RequisationForms.Where(m => m.FormStatus == FormStatus.PENDING_ADDITION || m.FormStatus == FormStatus.PENDING_RETRIVAL).Select(m => m.RequestedBy.DepartmentCode).Distinct().Count();
        }

        public int GetDisbursementListbyDepCode(string code)
        {
            return dbContext.DisbursementLists.Where(m => m.Department.DepartmentCode == code && ((m.DisbursementStatus == DisbursementStatus.WAITING_FOR_ACK || m.DisbursementStatus == DisbursementStatus.WAITING_FOR_OTP)) && m.DisburseItems.Count > 0).Count();
        }
    }
}
