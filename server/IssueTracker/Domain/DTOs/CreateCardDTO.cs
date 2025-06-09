using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CreateCardDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public List<string>? Labels { get; set; }
        
        [Required(ErrorMessage = "Item ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Item ID must be greater than 0")]
        public int ItemId { get; set; }
    }
}
