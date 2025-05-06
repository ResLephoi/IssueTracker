using System.Collections.Generic;

namespace IssueTracker.Models
{
    public class Board
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<Item> Items { get; set; }
    }
}
