using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Domain.Entities
{
    public class Card
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string>? Labels { get; set; }

        [ForeignKey("ItemId")]
        public int ItemId { get; set; }
        public Item Item { get; set; }
          [ForeignKey("AssignedToUserId")]
        public int? AssignedToUserId { get; set; }
        public SystemUser? AssignedToUser { get; set; }
    }
}
