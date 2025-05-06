using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Mock authentication logic
            if (request.Email == "test@example.com" && request.Password == "password")
            {
                return Ok(new { Token = "mock-jwt-token" });
            }

            return Unauthorized(new { Message = "Invalid credentials" });
        }
    }
}
