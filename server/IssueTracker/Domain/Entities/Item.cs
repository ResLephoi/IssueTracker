using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Domain.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [ForeignKey("BoardId")]
        public int BoardId { get; set; }
        public Board Board { get; set; }

        [InverseProperty("Item")]
        public List<Card>? Cards { get; set; }
    }
}
