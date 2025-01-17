using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Service
{
    public interface ITransactionService
    {
        Task<List<BorrowingTransaction>> GetAllLendTransactions();
        Task AddLendTransaction(BorrowingTransaction transaction);
        Task UpdateLendTransaction(BorrowingTransaction transaction);
        Task DeleteLendTransaction(int transactionId);
        Task UpdateReturnTransaction(int transactionId, DateTime returnDate);
    }
    public class TransactionService : ITransactionService   
    {
        private readonly LibraryDbContext _context;

        public TransactionService(LibraryDbContext context)
        {
            _context = context;
        }


        public async Task<List<BorrowingTransaction>> GetAllLendTransactions()
        {
            return await _context.BorrowingTransaction
                .Include(t => t.TransactionBooks)
                .ThenInclude(tb => tb.Book)
                .Include(t => t.Member)
                .ToListAsync();
        }

        // Add a new lending transaction
        public async Task AddLendTransaction(BorrowingTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");

            if (transaction.MemberID <= 0)
                throw new ArgumentException("Invalid Member ID.");

            if (transaction.TransactionBooks == null || !transaction.TransactionBooks.Any())
                throw new ArgumentException("At least one book must be included in the transaction.");

            // Check availability and update each book
            foreach (var transactionBook in transaction.TransactionBooks)
            {
                var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == transactionBook.BookID);

                if (book == null)
                    throw new InvalidOperationException($"Book with ID {transactionBook.BookID} does not exist.");

                if (!book.Availability)
                    throw new InvalidOperationException($"Book '{book.Title}' is not available for borrowing.");

                // Mark book as unavailable
                book.Availability = false;
            }

            _context.BorrowingTransaction.Add(transaction);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateLendTransaction(BorrowingTransaction updatedTransaction)
        {
            if (updatedTransaction == null)
                throw new ArgumentNullException(nameof(updatedTransaction), "Transaction cannot be null.");

            // Find the existing transaction
            var existingTransaction = await _context.BorrowingTransaction
                .Include(t => t.TransactionBooks)
                .FirstOrDefaultAsync(t => t.TransactionID == updatedTransaction.TransactionID);

            if (existingTransaction == null)
                throw new InvalidOperationException("Transaction not found.");

            // Get the current list of books in the transaction
            var existingBookIds = existingTransaction.TransactionBooks.Select(tb => tb.BookID).ToList();
            var updatedBookIds = updatedTransaction.TransactionBooks.Select(tb => tb.BookID).ToList();

            // Books to remove (set availability to true)
            var removedBookIds = existingBookIds.Except(updatedBookIds).ToList();
            foreach (var bookId in removedBookIds)
            {
                var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == bookId);
                if (book != null)
                {
                    book.Availability = true;
                }
            }

            // Books to add (set availability to false)
            var addedBookIds = updatedBookIds.Except(existingBookIds).ToList();
            foreach (var bookId in addedBookIds)
            {
                var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == bookId);
                if (book == null)
                    throw new InvalidOperationException($"Book with ID {bookId} does not exist.");

                if (!book.Availability)
                    throw new InvalidOperationException($"Book '{book.Title}' is not available for borrowing.");

                book.Availability = false;
            }

            // Update the transaction details
            existingTransaction.BorrowDate = updatedTransaction.BorrowDate;
            existingTransaction.DueDate = updatedTransaction.DueDate;
            existingTransaction.TransactionBooks = updatedTransaction.TransactionBooks;

            await _context.SaveChangesAsync();
        }

        // Delete an existing lending transaction
        public async Task DeleteLendTransaction(int transactionId)
        {
            // Find the existing transaction
            var transaction = await _context.BorrowingTransaction
                .Include(t => t.TransactionBooks)
                .FirstOrDefaultAsync(t => t.TransactionID == transactionId);

            if (transaction == null)
                throw new InvalidOperationException("Transaction not found.");

            // Set all books in the transaction as available
            foreach (var transactionBook in transaction.TransactionBooks)
            {
                var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == transactionBook.BookID);
                if (book != null)
                {
                    book.Availability = true;
                }
            }

            // Remove the transaction
            _context.BorrowingTransaction.Remove(transaction);
            await _context.SaveChangesAsync();
        }


        // Update a return transaction
        public async Task UpdateReturnTransaction(int transactionId, DateTime returnDate)
        {
            var transaction = await _context.BorrowingTransaction
                .Include(t => t.TransactionBooks)
                .FirstOrDefaultAsync(t => t.TransactionID == transactionId);

            if (transaction == null)
                throw new InvalidOperationException("Transaction not found.");

            transaction.ReturnDate = returnDate;

            // Update availability for all books in the transaction
            foreach (var transactionBook in transaction.TransactionBooks)
            {
                var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == transactionBook.BookID);

                if (book == null)
                    throw new InvalidOperationException($"Book with ID {transactionBook.BookID} does not exist.");

                // Mark book as available
                book.Availability = true;
            }

            await _context.SaveChangesAsync();
        }

    }
}
