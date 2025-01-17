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
        public int TransactionID { get; set; }  // Primary Key
        public ICollection<TransactionBook> TransactionBooks { get; set; }

        [Required]
        public int MemberID { get; set; }  // Foreign Key to Member table
        public Member Member { get; set; }  // Navigation property

        [Required]
        public DateTime BorrowDate { get; set; }  // Date when the book was borrowed

        [Required]
        public DateTime DueDate { get; set; }  // Expected return date

        public DateTime? ReturnDate { get; set; }  // Actual return date (nullable)
    }
}
