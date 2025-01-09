using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace LibraryManagementSystem.Models
{
    class LibraryDbContext : DbContext
    {
        public LibraryDbContext() : base("name=LibraryDBConnectionString") { }
    }
}
