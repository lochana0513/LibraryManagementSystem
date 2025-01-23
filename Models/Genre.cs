using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Genre
    {
        [Key]
        public int GenreID { get; set; }  

        [Required]
        [MaxLength(255)]
        public string GenreName { get; set; }  

        public ICollection<Book> Book{ get; set; }
    }
}
