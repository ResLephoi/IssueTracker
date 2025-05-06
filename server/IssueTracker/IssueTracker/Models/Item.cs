namespace IssueTracker.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<Card> Cards { get; set; }
    }
}
