using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CreateItemDTO
    {
        public string Title { get; set; }

        public int BoardId { get; set; }
    }
}
