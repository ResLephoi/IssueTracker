using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using IssueTracker.Application.Services;
using IssueTracker.Domain.Interfaces;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.DTOs;
using Microsoft.Extensions.Configuration;

namespace IssueTrackerTests.ServicesTests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IAuthRepository> _mockAuthRepository = null!;
        private Mock<IConfiguration> _mockConfiguration = null!;
        private AuthService _authService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAuthRepository = new Mock<IAuthRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Setup JWT configuration section
            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(x => x["Key"]).Returns("MySecretKeyForJWTTokenGenerationThatIsAtLeast32CharactersLong!");
            jwtSection.Setup(x => x["Issuer"]).Returns("IssueTracker");
            jwtSection.Setup(x => x["Audience"]).Returns("IssueTrackerUsers");
            jwtSection.Setup(x => x["ExpiresInMinutes"]).Returns("60");
            
            _mockConfiguration.Setup(x => x.GetSection("Jwt")).Returns(jwtSection.Object);
            
            _authService = new AuthService(_mockAuthRepository.Object, _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var user = new SystemUser
            {
                Id = 1,
                Username = "testuser",
                Password = PasswordHasher.HashPassword("testpassword"),
                IsActive = true
            };

            _mockAuthRepository.Setup(repo => repo.GetUserByUsernameAsync("testuser"))
                              .ReturnsAsync(user);

            _mockAuthRepository.Setup(repo => repo.UpdateLastLoginAsync(user))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Token.Should().NotBeNullOrEmpty();
            result.Token.Should().Contain("eyJ"); // JWT tokens start with eyJ when base64 encoded
            result.Username.Should().Be("testuser");

            _mockAuthRepository.Verify(repo => repo.GetUserByUsernameAsync("testuser"), Times.Once);
            _mockAuthRepository.Verify(repo => repo.UpdateLastLoginAsync(user), Times.Once);
        }

        [TestMethod]
        public async Task AuthenticateAsync_EmptyUsername_ReturnsFailureResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "",
                Password = "testpassword"
            };

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Username and password are required");

            _mockAuthRepository.Verify(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task AuthenticateAsync_EmptyPassword_ReturnsFailureResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO
            {
                Username = "testuser",
                Password = ""
            };

            // Act
            var result = await _authService.AuthenticateAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Username and password are required");

            _mockAuthRepository.Verify(repo => repo.GetUserByUsernameAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
