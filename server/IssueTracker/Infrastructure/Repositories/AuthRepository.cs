using System;
using System.Linq;
using System.Threading.Tasks;
using IssueTracker.Domain.DTOs;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Interfaces;
using IssueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IssueTrackerDbContext _context;

        public AuthRepository(IssueTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetUsersRequestDTO>> GetAllUsersAsync()
        {
            return await _context.SystemUsers
                .Where(u => u.IsActive)
                .Select(u => new GetUsersRequestDTO
                {
                    Id = u.Id,
                    Username = u.Username
                }).ToListAsync();
        }

        public async Task<SystemUser> GetUserByUsernameAsync(string username)
        {
            return await _context.SystemUsers
                .Where(u => u.Username == username && u.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateLastLoginAsync(SystemUser user)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
