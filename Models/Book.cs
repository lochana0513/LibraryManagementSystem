using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }  // Primary Key
        public ICollection<TransactionBook> TransactionBooks { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }  // Title of the book

        [Required]
        [MaxLength(255)]
        public string Author { get; set; }  // Author's name

        [MaxLength(13)]
        public string ISBN { get; set; }  // ISBN of the book (optional)

        [Required]
        public int GenreID { get; set; }  // Foreign Key to Genre table
        public Genre Genre { get; set; }  // Navigation property

        [Required]
        public bool Availability { get; set; }  // Whether the book is available for borrowing
    }
}
