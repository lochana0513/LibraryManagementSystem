﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Service
{
    public interface IGenreService
    {
        Task<List<Genre>> GetAllGenre();
        Task AddGenre(Genre genre);
        Task UpdateGenre(Genre genre);
    }
    public class GenreService : IGenreService
    {
        private readonly LibraryDbContext _context;

        public GenreService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Genre>> GetAllGenre()
        {
            return await _context.Genre.ToListAsync();
        }

        public async Task AddGenre(Genre genre)
        {
            if (genre == null)
                throw new ArgumentNullException(nameof(genre), "Genre cannot be null.");

            if (string.IsNullOrWhiteSpace(genre.GenreName))
                throw new ArgumentException("Genre name is required.");

            var existingGenre = await _context.Genre.FirstOrDefaultAsync(g => g.GenreName == genre.GenreName);
            if (existingGenre != null)
                throw new InvalidOperationException("A genre with the same name already exists.");

            _context.Genre.Add(genre);
            await _context.SaveChangesAsync();  
        }

        public async Task UpdateGenre(Genre genre)
        {
            var existingGenre = await _context.Genre.FirstOrDefaultAsync(g => g.GenreID == genre.GenreID);
            if (existingGenre != null)
            {
                existingGenre.GenreName = genre.GenreName;

                await _context.SaveChangesAsync(); 
            }
        }


    }
}
