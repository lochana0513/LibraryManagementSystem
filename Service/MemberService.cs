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

        // Get all members
        public async Task<List<Member>> GetAllMembers()
        {
            return await _context.Member.ToListAsync(); // Fetch all members asynchronously
        }

        // Add a new member
        public async Task AddMember(Member member)
        {
            // Validation: Ensure the member object is not null
            if (member == null)
                throw new ArgumentNullException(nameof(member), "Member cannot be null.");

            // Validation: Check required fields
            if (string.IsNullOrWhiteSpace(member.Name))
                throw new ArgumentException("Member name is required.");
            if (string.IsNullOrWhiteSpace(member.ContactInfo))
                throw new ArgumentException("Member contact info is required.");

            // Optional: Ensure ContactInfo is unique (e.g., email or phone number)
            var existingMember = await _context.Member
                .FirstOrDefaultAsync(m => m.ContactInfo == member.ContactInfo);
            if (existingMember != null)
                throw new InvalidOperationException("A member with the same contact information already exists.");

            // Add the member to the database
            _context.Member.Add(member);
            await _context.SaveChangesAsync(); // Save changes asynchronously
        }

        // Update an existing member
        public async Task UpdateMember(Member member)
        {
            // Validation: Ensure the member object is not null
            if (member == null)
                throw new ArgumentNullException(nameof(member), "Member cannot be null.");

            // Fetch the existing member by ID
            var existingMember = await _context.Member.FirstOrDefaultAsync(m => m.MemberID == member.MemberID);
            if (existingMember == null)
                throw new KeyNotFoundException("The specified member does not exist.");

            // Update the member's properties
            existingMember.Name = member.Name;
            existingMember.ContactInfo = member.ContactInfo;
            existingMember.MembershipDate = member.MembershipDate;

            // Save changes asynchronously
            await _context.SaveChangesAsync();
        }

        // Delete a member
        public async Task DeleteMember(int MemberID)
        {
            // Fetch the member by ID
            var member = await _context.Member.FirstOrDefaultAsync(m => m.MemberID == MemberID);
            if (member == null)
                throw new KeyNotFoundException("The specified member does not exist.");

            // Remove the member from the database
            _context.Member.Remove(member);
            await _context.SaveChangesAsync(); // Save changes asynchronously
        }

    }
}
