namespace IssueTracker.Models
{
    public class Card
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Labels { get; set; }
    }
}
