using OfficeOpenXml;
using SSIS.Models;
using SSIS.Security.Filter;
using SSIS.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;

namespace SSIS.Controllers
{
    /*
     *Coded by
     * May Thandar Theint Aung
     * Yao Hai
     */
    [Authorize(Roles = "STORE_CLERK, STORE_SUPERVISOR, STORE_MANAGER")]
    [SessionCheck]
    public class ReportController : Controller
    {
        private SSISDbContext dbContext = new SSISDbContext();

        private ItemServices itemServices;
        public ReportController()
        {
            itemServices = new ItemServices(dbContext);
        }

        public void ItemInventoryReport()
        {
            List<Item> itemList = itemServices.GetItems();

            ExcelPackage pck = new ExcelPackage();

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Logic University";
            ws.Cells["B1"].Value = " ";

            ws.Cells["A2"].Value = "Store";
            ws.Cells["B2"].Value = "Item Report";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:HH: mm}", DateTimeOffset.Now);

            ws.Cells["A6"].Value = "Item Code";
            ws.Cells["B6"].Value = "Category";
            ws.Cells["C6"].Value = "Description";
            ws.Cells["D6"].Value = "Reorder Level";
            ws.Cells["E6"].Value = "Reorder Quantity";
            ws.Cells["F6"].Value = "Current Quantity";
            ws.Cells["G6"].Value = "Unit Of Measure";

            int rowStart = 7;
            foreach (var item in itemList)
            {
                if (item.CurrentQuantity < item.ReorderLevel)
                {
                    ws.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));
                }
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.ItemCode;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Category;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.Description;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.ReorderLevel;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.ReorderQuantity;
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.CurrentQuantity;
                ws.Cells[string.Format("G{0}", rowStart)].Value = item.UnitOfMeasure;

                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ItemInvenroryReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        public void ReorderReport()
        {
            List<PurchaseOrder> purchasorderList = itemServices.GetAllPurchaseOrders();
            ExcelPackage pck = new ExcelPackage();

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Reoder");

            ws.Cells["A1"].Value = "Logic University";
            ws.Cells["B1"].Value = " ";

            ws.Cells["A2"].Value = "Store";
            ws.Cells["B2"].Value = "Reorder Report";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:HH: mm}", DateTimeOffset.Now);

            ws.Cells["A6"].Value = "Item Code";
            ws.Cells["B6"].Value = "Description";
            ws.Cells["C6"].Value = "Quantity On Hand";
            ws.Cells["D6"].Value = "Reorder Level";
            ws.Cells["E6"].Value = "Reorder Quantity";
            ws.Cells["F6"].Value = "Purchase Order Number";
            ws.Cells["G6"].Value = "Expected Delivery";

            int rowStart = 7;
            foreach (var order in purchasorderList)
            {
                foreach (var item in order.PurchaseItems)
                {
                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.Item.ItemCode;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.Item.Description;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.Item.CurrentQuantity;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.Item.ReorderLevel;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.Quantity;
                    ws.Cells[string.Format("F{0}", rowStart)].Value = order.PurchaseOrderNumber;
                    ws.Cells[string.Format("G{0}", rowStart)].Value = order.DeliveredDate.ToString();
                    rowStart++;
                }
            }
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "reorder.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        public void GenerateReorderReport(string fromdate, string todate, string selected)
        {
            List<PurchaseOrder> purchasorderList = new List<PurchaseOrder>();
            DateTime FromDate = DateTime.Parse(fromdate);
            DateTime ToDate = DateTime.Parse(todate);
            if (ToDate == DateTime.Today)
            {
                ToDate = DateTime.Now;
            }
            int status = int.Parse(selected);

            if (status != 2)
            {
                //purchasorderList = dbContext.PurchaseOrders.Include(m => m.PurchaseItems.Select(a => a.Item)).Where(m => m.PurchaseOrderStatus == (Enums.PurchaseOrderStatus)status).Where(m => m.PurchaseOrderDate <= ToDate && m.PurchaseOrderDate >= FromDate).ToList();
                purchasorderList = itemServices.GetPurchaseOrdersBySelectedDateAndStatus(FromDate, ToDate, status);
            }
            else
            {
                //purchasorderList = dbContext.PurchaseOrders.Include(m => m.PurchaseItems.Select(a => a.Item)).Where(m => m.PurchaseOrderDate >= FromDate && m.PurchaseOrderDate <= ToDate).ToList();
                purchasorderList = itemServices.GetAllPurchaseOrdersBySelectedDate(FromDate, ToDate);
            }
            ExcelPackage pck = new ExcelPackage();

            //This line add a new Excel
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ReOrder");

            ws.Cells["A1"].Value = "Logic University";
            ws.Cells["B1"].Value = " ";

            ws.Cells["A2"].Value = "Store";
            ws.Cells["B2"].Value = "Reorder Report";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:HH: mm}", DateTimeOffset.Now);

            ws.Cells["A6"].Value = "Item Code";
            ws.Cells["B6"].Value = "Description";
            ws.Cells["C6"].Value = "Quantity On Hand";
            ws.Cells["D6"].Value = "Reorder Level";
            ws.Cells["E6"].Value = "Ordered Quantity";
            ws.Cells["F6"].Value = "Purchase Order Number";

            int rowStart = 7;
            foreach (var order in purchasorderList)
            {
                foreach (var item in order.PurchaseItems)
                {
                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.Item.ItemCode;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.Item.Description;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.Item.CurrentQuantity;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.Item.ReorderLevel;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.Quantity;
                    ws.Cells[string.Format("F{0}", rowStart)].Value = order.PurchaseOrderNumber;
                    rowStart++;
                }
            }
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "reorder.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }
    }
}
