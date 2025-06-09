using Domain.DTOs;
using IssueTracker.Application.Services;
using IssueTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ItemController(ItemService itemService)
        {
            _itemService = itemService;
        }        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(item);
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
                var items = await _itemService.GetAllItemsAsync();
                return Ok(items);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }        [HttpPost]
        public async Task<IActionResult> Create(CreateItemDTO createItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var item = await _itemService.AddItemAsync(createItemDto);
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }[HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Item item)
        {
            try
            {
                if (id != item.Id) return BadRequest();
                await _itemService.UpdateItemAsync(item);
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
                await _itemService.DeleteItemAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}