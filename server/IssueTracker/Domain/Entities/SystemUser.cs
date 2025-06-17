using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Domain.Entities
{
    public class SystemUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }
}
