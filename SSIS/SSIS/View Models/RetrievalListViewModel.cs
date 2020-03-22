using System.Collections.Generic;

namespace SSIS.View_Models
{
    public class RetrievalListViewModel
    {
        public string RequisationFormsID { get; set; }
        public List<RetrievalList> RetrivalLists { get; set; }
        public bool ErrorFlag { get; set; }
    }
}
