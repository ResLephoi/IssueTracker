using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Domain.DTOs
{
    public class GetUsersRequestDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
    }
}
