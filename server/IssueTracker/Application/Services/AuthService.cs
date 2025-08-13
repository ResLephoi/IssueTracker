using IssueTracker.Domain.DTOs;
using IssueTracker.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IssueTracker.Application.Services
{
    public class AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IConfiguration _configuration = configuration;

        public async Task<LoginResponseDTO> AuthenticateAsync(LoginRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Username and password are required"
                };
            }

            var user = await _authRepository.GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            if (!PasswordHasher.VerifyPassword(request.Password, user.Password))
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            await _authRepository.UpdateLastLoginAsync(user);

            var token = GenerateJwtToken(user.Username, user.Id.ToString());

            return new LoginResponseDTO
            {
                Success = true,
                Token = token,
                Username = user.Username
            };
        }

        private string GenerateJwtToken(string username, string userId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"] ?? "60"));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim("userId", userId),
                new Claim("username", username)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IEnumerable<GetUsersRequestDTO>> GetUsersAsync()
        {
            return await _authRepository.GetAllUsersAsync();
        }
    }
}
