using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class TransactionBook
    {
        [Required]
        public int TransactionID { get; set; }  // Foreign Key to BorrowingTransaction table
        public BorrowingTransaction BorrowingTransaction { get; set; }  // Navigation property

        [Required]
        public int BookID { get; set; }  // Foreign Key to Book table
        public Book Book { get; set; }  // Navigation property
    }
}
