using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Domain.Entities
{
    public class Board
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [InverseProperty("Board")]
        public List<Item>? Items { get; set; }
    }
}
