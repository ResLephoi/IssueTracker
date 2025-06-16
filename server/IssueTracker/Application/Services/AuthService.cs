using IssueTracker.Domain.DTOs;
using IssueTracker.Domain.Interfaces;

namespace IssueTracker.Application.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

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

            return new LoginResponseDTO
            {
                Success = true,
                Token = "mock-jwt-token",
                Username = user.Username
            };
        }
    }
}
