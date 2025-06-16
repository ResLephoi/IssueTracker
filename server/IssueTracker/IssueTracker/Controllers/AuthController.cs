using Microsoft.AspNetCore.Mvc;
using IssueTracker.Domain.DTOs;
using IssueTracker.Application.Services;

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
    }
}
