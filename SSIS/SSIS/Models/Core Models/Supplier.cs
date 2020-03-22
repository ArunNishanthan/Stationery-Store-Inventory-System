using System;
using System.ComponentModel.DataAnnotations;

namespace SSIS.Models
{
    public class Supplier
    {
        [Key]
        public String SupplierCode { get; set; }
        public String SupplierName { get; set; }
        public String PhoneNumber { get; set; }
        public String ContactName { get; set; }

        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string GSTNumber { get; set; }
    }
}
