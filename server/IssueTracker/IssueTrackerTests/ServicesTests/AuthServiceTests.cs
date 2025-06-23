using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using IssueTracker.Application.Services;
using IssueTracker.Domain.Interfaces;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.DTOs;

namespace IssueTrackerTests.ServicesTests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IAuthRepository> _mockAuthRepository = null!;
        private AuthService _authService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAuthRepository = new Mock<IAuthRepository>();
            _authService = new AuthService(_mockAuthRepository.Object);
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
            result.Token.Should().Be("mock-jwt-token");
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
