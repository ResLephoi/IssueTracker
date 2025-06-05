using Domain.DTOs;
using IssueTracker.Application.Services;
using IssueTracker.Domain.Entities;
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
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null) return NotFound();
            return Ok(card);
        }

        [HttpGet("GetAllCards")]
        public async Task<IActionResult> GetAll()
        {
            var cards = await _cardService.GetAllCardsAsync();
            return Ok(cards);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCardDTO cardDto)
        {
            var card = await _cardService.AddCardAsync(cardDto);
            return CreatedAtAction(nameof(GetById), new { id = card.Id }, card);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateCardDTO updateCardDTO)
        {
            if (updateCardDTO.Id ==0) return BadRequest();
            await _cardService.UpdateCardAsync(updateCardDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _cardService.DeleteCardAsync(id);
            return NoContent();
        }
    }
}
