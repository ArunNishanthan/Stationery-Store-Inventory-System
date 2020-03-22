using System.Collections.Generic;

namespace SSIS.View_Models
{
    public class SystemGeneratePurchaseOrderViewModel
    {
        public string RequestCode { get; set; }
        public string DeleteCode { get; set; }
        public List<PurchaseOrderQuantity> PurchaseOrderQuantityList { get; set; }
    }
}
