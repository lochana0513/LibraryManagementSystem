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
        public int BookID { get; set; } 
        public ICollection<TransactionBook> TransactionBooks { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }  

        [Required]
        [MaxLength(255)]
        public string Author { get; set; }  
        [MaxLength(13)]
        public string ISBN { get; set; }  

        [Required]
        public int GenreID { get; set; }  
        public Genre Genre { get; set; } 

        [Required]
        public bool Availability { get; set; } 
    }
}
