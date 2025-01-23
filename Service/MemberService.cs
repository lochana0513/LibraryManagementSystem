using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Service
{
    public interface IMemberService
    {
        Task<List<Member>> GetAllMembers();
        Task<List<Member>> GetAllAvaiableMembers();
        Task AddMember(Member member);
        Task UpdateMember(Member member);
        Task DeleteMember(int MemberID);
    }
    public class MemberService : IMemberService
    {
        private readonly LibraryDbContext _context;

        public MemberService(LibraryDbContext context)
        {
            _context = context;
        }

     
        public async Task<List<Member>> GetAllMembers()
        {
            return await _context.Member.ToListAsync(); 
        }

        public async Task<List<Member>> GetAllAvaiableMembers()
        {
            
            var availableMembers = await _context.Member
                .Where(member => !_context.BorrowingTransaction
                    .Any(transaction => transaction.MemberID == member.MemberID && transaction.ReturnDate == null))
                .ToListAsync();

            return availableMembers;
        }


        public async Task AddMember(Member member)
        {
      
            if (member == null)
                throw new ArgumentNullException(nameof(member), "Member cannot be null.");

            
            if (string.IsNullOrWhiteSpace(member.Name))
                throw new ArgumentException("Member name is required.");

            if (string.IsNullOrWhiteSpace(member.ContactInfo))
                throw new ArgumentException("Member contact info is required.");

            var existingMember = await _context.Member
                .FirstOrDefaultAsync(m => m.ContactInfo == member.ContactInfo);
            if (existingMember != null)
                throw new InvalidOperationException("A member with the same contact information already exists.");

            _context.Member.Add(member);
            await _context.SaveChangesAsync(); 
        }

        
        public async Task UpdateMember(Member member)
        {
            
            if (member == null)
                throw new ArgumentNullException(nameof(member), "Member cannot be null.");

            
            var existingMember = await _context.Member.FirstOrDefaultAsync(m => m.MemberID == member.MemberID);
            if (existingMember == null)
                throw new KeyNotFoundException("The specified member does not exist.");

            
            existingMember.Name = member.Name;
            existingMember.ContactInfo = member.ContactInfo;
            existingMember.MembershipDate = member.MembershipDate;

            
            await _context.SaveChangesAsync();
        }


        public async Task DeleteMember(int MemberID)
        {
            
            var member = await _context.Member.FirstOrDefaultAsync(m => m.MemberID == MemberID);
            if (member == null)
                throw new KeyNotFoundException("The specified member does not exist.");

            
            _context.Member.Remove(member);
            await _context.SaveChangesAsync(); 
        }

    }
}
