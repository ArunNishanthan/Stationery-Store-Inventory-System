using SSIS.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SSIS.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String Password { get; set; }
        public Role Role { get; set; }
    }

}
