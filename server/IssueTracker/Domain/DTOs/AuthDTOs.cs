using System;
using System.Threading.Tasks;

namespace IssueTracker.Domain.DTOs
{
    public class LoginRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
