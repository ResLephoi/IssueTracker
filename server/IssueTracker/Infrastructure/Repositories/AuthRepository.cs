using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<LoginRequest> GetUserByUsernameAsync(string username)
        {
            return await _context.LoginRequests
                .Where(u => u.Username == username && u.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateLastLoginAsync(LoginRequest user)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
