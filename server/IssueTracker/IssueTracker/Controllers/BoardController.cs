using IssueTracker.Application.Services;
using IssueTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly BoardService _boardService;

        public BoardController(BoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);
            if (board == null) return NotFound();
            return Ok(board);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var boards = await _boardService.GetAllBoardsAsync();
            return Ok(boards);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Board board)
        {
            await _boardService.AddBoardAsync(board);
            return CreatedAtAction(nameof(GetById), new { id = board.Id }, board);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Board board)
        {
            if (id != board.Id) return BadRequest();
            await _boardService.UpdateBoardAsync(board);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _boardService.DeleteBoardAsync(id);
            return NoContent();
        }
    }
}
