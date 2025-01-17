using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Member
    {
        [Key]
        public int MemberID { get; set; }  // Primary Key

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }  // Full name of the member

        [Required]
        [MaxLength(255)]
        public string ContactInfo { get; set; }  // Contact info (phone number or email)

        [Required]
        public DateTime MembershipDate { get; set; }  // Date when membership started
    }
}
