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
        public int UserID { get; set; }  // Primary Key

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }  // Email (unique)

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }  // Password (hashed)

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }  // User's first name

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }  // User's last name

        [MaxLength(20)]
        public string PhoneNumber { get; set; }  // Optional phone number

        [MaxLength(20)]
        public string NICNumber { get; set; }  // Optional NIC number
    }
}
