using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }  

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }  

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }  

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }  

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }  

        [MaxLength(20)]
        public string PhoneNumber { get; set; }  

        [MaxLength(20)]
        public string NICNumber { get; set; } 
    }
}
