using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Pages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Models
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Book { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<BorrowingTransaction> BorrowingTransaction { get; set; }
        public DbSet<TransactionBook> TransactionBook { get; set; }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public void ApplyMigrations() { Database.Migrate(); }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<TransactionBook>()
                .HasKey(tb => new { tb.TransactionID, tb.BookID });

            

            base.OnModelCreating(modelBuilder);
        }
    }
}
