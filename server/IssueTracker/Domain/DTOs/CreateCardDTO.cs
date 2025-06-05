using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CreateCardDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string>? Labels { get; set; }
        public int ItemId { get; set; }
    }
}
