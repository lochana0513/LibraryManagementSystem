﻿using System;
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
        Task<List<BorrowingTransaction>> GetUnreturnedBorrowTransactions();
        Task AddLendTransaction(BorrowingTransaction transaction, List<TransactionBook> transactionBooks);

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

        public async Task<List<BorrowingTransaction>> GetUnreturnedBorrowTransactions()
        {
            return await _context.BorrowingTransaction
                .Where(t => t.ReturnDate == null) // Filter for unreturned books
                .Include(t => t.TransactionBooks)
                .ThenInclude(tb => tb.Book)
                .Include(t => t.Member)
                .ToListAsync();
        }

        // Add a new lending transaction
        public async Task AddLendTransaction(BorrowingTransaction transaction, List<TransactionBook> transactionBooks)
        {
            // Validate the transaction object
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");

            if (transaction.MemberID <= 0)
                throw new ArgumentException("Invalid Member ID.");

            if (transactionBooks == null || !transactionBooks.Any())
                throw new ArgumentException("At least one book must be included in the transaction.");

            if (transaction.DueDate <= DateTime.Now)
                throw new ArgumentException("Due date must be a future date.");

            // Validate each book and update availability
            foreach (var transactionBook in transactionBooks)
            {
                var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == transactionBook.BookID);

                if (book == null)
                    throw new InvalidOperationException($"Book with ID {transactionBook.BookID} does not exist.");

                if (!book.Availability)
                    throw new InvalidOperationException($"Book '{book.Title}' is not available for borrowing.");

                // Mark the book as unavailable
                book.Availability = false;

                // Link the transactionBook to the transaction
                transactionBook.TransactionID = transaction.TransactionID;
                _context.TransactionBook.Add(transactionBook);
            }

            // Add the transaction
            _context.BorrowingTransaction.Add(transaction);

            // Save changes to the database
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
