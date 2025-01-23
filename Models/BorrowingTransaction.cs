using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class BorrowingTransaction
    {
        [Key]
        public int TransactionID { get; set; }  
        public ICollection<TransactionBook> TransactionBooks { get; set; }

        [Required]
        public int MemberID { get; set; }  
        public Member Member { get; set; }  

        [Required]
        public DateTime BorrowDate { get; set; }  

        [Required]
        public DateTime DueDate { get; set; } 

        public DateTime? ReturnDate { get; set; }  
    }
}
