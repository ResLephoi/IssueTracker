using IssueTracker.Domain.DTOs;
using IssueTracker.Application.Services;
using IssueTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IssueTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BoardController : ControllerBase
    {
        private readonly BoardService _boardService;

        public BoardController(BoardService boardService)
        {
            _boardService = boardService;
        }        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var board = await _boardService.GetBoardByIdAsync(id);
                if (board == null) return NotFound();
                return Ok(board);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetBoardWithItemsAndCards(int id)
        {
            try
            {
                var board = await _boardService.GetBoardWithItemsAndCardsAsync(id);
                if (board == null) return NotFound();
                return Ok(board);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var boards = await _boardService.GetAllBoardsAsync();
                return Ok(boards);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }        [HttpPost]
        public async Task<IActionResult> Create(CreateBoardDTO createBoardDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var board = await _boardService.AddBoardAsync(createBoardDto);
                return CreatedAtAction(nameof(GetById), new { id = board.Id }, board);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }[HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Board board)
        {
            try
            {
                if (id != board.Id) return BadRequest();
                await _boardService.UpdateBoardAsync(board);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _boardService.DeleteBoardAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
