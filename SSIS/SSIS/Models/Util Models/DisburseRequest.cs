using System.Collections.Generic;

namespace SSIS.Models.Util_Models
{
    public class DisburseRequest
    {
        public int Id { get; set; }
        public List<RequestItem> RequestItems { get; set; }
    }
}
