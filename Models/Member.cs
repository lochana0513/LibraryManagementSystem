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
        public int MemberID { get; set; }  

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }  
        [Required]
        [MaxLength(255)]
        public string ContactInfo { get; set; }  

        [Required]
        public DateTime MembershipDate { get; set; }  
    }
}
