using SSIS.Enums;

namespace SSIS.Models
{
    public class RequestForm
    {
        public int Id { get; set; }
        public string Comments { get; set; }
        public FormStatus FormStatus { get; set; }
        public int HandledBy { get; set; }
    }
}
