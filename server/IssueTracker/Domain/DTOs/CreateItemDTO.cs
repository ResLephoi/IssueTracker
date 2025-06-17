using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Domain.DTOs
{
    public class CreateItemDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 150 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Board ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Board ID must be greater than 0")]
        public int BoardId { get; set; }
    }
}
