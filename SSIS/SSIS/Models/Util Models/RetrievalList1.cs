using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSIS.View_Models
{
    public class RetrievalList1
    {
        public string RetrievedItem { get; set; }
        public int Needed { get; set; }
        public int Retrieved { get; set; }
        public List<Retrieval1> Retrievals { get; set; }
    }
}