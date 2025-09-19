using Microsoft.AspNetCore.Mvc;
using IssueTracker.Domain.DTOs;
using IssueTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace IssueTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var result = await _authService.AuthenticateAsync(request);

            if (!result.Success)
            {
                if (result.Message == "Username and password are required")
                {
                    return BadRequest(new { Message = result.Message });
                }
                
                return Unauthorized(new { Message = result.Message });
            }

            return Ok(new { result.Success, result.Token, result.Username });
        }

        [HttpGet("GetUsers")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authService.GetUsersAsync();
            if (users == null || !users.Any())
            {
                return NotFound(new { Message = "No users found" });
            }

            return Ok(users);
        }

        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                          User.FindFirst("sub")?.Value;
            
            return Ok(new { 
                Message = "Token is valid",  
                Username = username,
                Timestamp = DateTime.UtcNow 
            });
        }
    }
}
