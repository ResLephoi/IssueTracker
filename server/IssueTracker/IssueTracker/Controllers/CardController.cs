using IssueTracker.Domain.DTOs;
using IssueTracker.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly CardService _cardService;

        public CardController(CardService cardService)
        {
            _cardService = cardService;
        }        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var card = await _cardService.GetCardByIdAsync(id);
                if (card == null) return NotFound();
                return Ok(card);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }        [HttpGet("GetAllCards")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var cards = await _cardService.GetAllCardsAsync();
                return Ok(cards);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }        [HttpPost]
        public async Task<IActionResult> Create(CreateCardDTO cardDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var card = await _cardService.AddCardAsync(cardDto);
                return CreatedAtAction(nameof(GetById), new { id = card.Id }, card);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }[HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateCardDTO updateCardDTO)
        {
            try
            {
                if (updateCardDTO.Id ==0) return BadRequest();
                await _cardService.UpdateCardAsync(updateCardDTO);
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
                await _cardService.DeleteCardAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
