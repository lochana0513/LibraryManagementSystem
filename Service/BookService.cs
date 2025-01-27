﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Pages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Service
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooks();
        Task<List<Book>> GetAllAvailableBooks();
        Task AddBook(Book book);
        Task UpdateBook(Book book);
        Task DeleteBook(int BookID);
    }
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }


        public async Task<List<Book>> GetAllBooks()
        {
            return await _context.Book.ToListAsync();

        }

        public async Task<List<Book>> GetAllAvailableBooks()
        {
            return await _context.Book
                .Where(b => b.Availability) 
                .ToListAsync();
        }



        public async Task AddBook(Book book)
        {

            if (book == null)
                throw new ArgumentNullException(nameof(book), "Book cannot be null.");

            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title is required.");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Book author is required.");

            if (string.IsNullOrWhiteSpace(book.ISBN))
                throw new ArgumentException("Book ISBN is required.");

            var existingBook = await _context.Book.FirstOrDefaultAsync(b => b.ISBN == book.ISBN);
            if (existingBook != null)
                throw new InvalidOperationException("A book with the same ISBN already exists.");

          
            book.Availability = true;
            _context.Book.Add(book);
            await _context.SaveChangesAsync();  
        }



        public async Task UpdateBook(Book book)
        {
            var existingBook = await _context.Book.FirstOrDefaultAsync(b => b.BookID == book.BookID);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                existingBook.Genre = book.Genre;

                await _context.SaveChangesAsync();  
            }
        }


        public async Task DeleteBook(int BookID)
        {
            var book = await _context.Book.FirstOrDefaultAsync(b => b.BookID == BookID);
            if (book != null)
            {
                _context.Book.Remove(book);
                await _context.SaveChangesAsync();  
            }
        }
    }
}
