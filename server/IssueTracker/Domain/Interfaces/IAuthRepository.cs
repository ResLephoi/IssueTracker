using System;
using System.Threading.Tasks;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<LoginRequest> GetUserByUsernameAsync(string username);
        Task UpdateLastLoginAsync(LoginRequest user);
    }
}
