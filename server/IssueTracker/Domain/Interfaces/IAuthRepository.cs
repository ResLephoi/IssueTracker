using System;
using System.Threading.Tasks;
using IssueTracker.Domain.DTOs;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<SystemUser> GetUserByUsernameAsync(string username);
        Task UpdateLastLoginAsync(SystemUser user);
        Task<IEnumerable<GetUsersRequestDTO>> GetAllUsersAsync();
    }
}
