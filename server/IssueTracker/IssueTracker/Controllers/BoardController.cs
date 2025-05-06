using IssueTracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardController : ControllerBase
    {
        private static readonly List<Board> Boards = new();

        [HttpGet]
        public IActionResult GetBoards()
        {
            return Ok(Boards);
        }

        [HttpPost]
        public IActionResult CreateBoard([FromBody] Board board)
        {
            Boards.Add(board);
            return CreatedAtAction(nameof(GetBoards), board);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBoard(string id, [FromBody] Board updatedBoard)
        {
            var board = Boards.Find(b => b.Id == id);
            if (board == null) return NotFound();

            board.Title = updatedBoard.Title;
            board.Items = updatedBoard.Items;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBoard(string id)
        {
            var board = Boards.Find(b => b.Id == id);
            if (board == null) return NotFound();

            Boards.Remove(board);
            return NoContent();
        }

        [HttpPost("{boardId}/lists/{listId}/cards")]
        public IActionResult AddCard(string boardId, string listId, [FromBody] Card newCard)
        {
            var board = Boards.Find(b => b.Id == boardId);
            if (board == null) return NotFound();

            var list = board.Items.Find(l => l.Id == listId);
            if (list == null) return NotFound();

            newCard.Id = Guid.NewGuid().ToString(); // Generate a unique ID for the card
            list.Cards.Add(newCard);

            return CreatedAtAction(nameof(GetBoards), newCard);
        }
    }
}
