using SSIS.Models;
using System.Collections.ObjectModel;

namespace SSIS.View_Models
{
    public class RequisitionViewModel
    {
        public Collection<RequestItem> RequestItems { get; set; }
        public string RequestCode { get; set; }
        public string DeleteCode { get; set; }
        public int RequisationFormID { get; set; }
    }
}
