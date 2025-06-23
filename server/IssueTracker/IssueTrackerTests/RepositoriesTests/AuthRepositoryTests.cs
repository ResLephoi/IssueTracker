using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Infrastructure.Data;
using IssueTracker.Infrastructure.Repositories;
using IssueTracker.Domain.Entities;
using FluentAssertions;
using System;

namespace IssueTrackerTests.RepositoriesTests
{
    [TestClass]
    public class AuthRepositoryTests
    {
        private IssueTrackerDbContext _context = null!;
        private AuthRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<IssueTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new IssueTrackerDbContext(options);
            _repository = new AuthRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetUserByUsernameAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var user = new SystemUser
            {
                Id = 1,
                Username = "testuser",
                Password = "hashedpassword",
                IsActive = true
            };
            await _context.SystemUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByUsernameAsync("testuser");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Username.Should().Be("testuser");
        }

        [TestMethod]
        public async Task GetUserByUsernameAsync_NonExistingUser_ReturnsNull()
        {
            // Act
            var result = await _repository.GetUserByUsernameAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetUserByUsernameAsync_InactiveUser_ReturnsNull()
        {
            // Arrange
            var user = new SystemUser
            {
                Id = 1,
                Username = "inactiveuser",
                Password = "hashedpassword",
                IsActive = false
            };
            await _context.SystemUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByUsernameAsync("inactiveuser");

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task UpdateLastLoginAsync_ExistingUser_UpdatesLastLoginTimestamp()
        {
            // Arrange
            var user = new SystemUser
            {
                Id = 1,
                Username = "testuser",
                Password = "hashedpassword",
                IsActive = true,
                LastLoginAt = null
            };
            await _context.SystemUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UpdateLastLoginAsync(user);

            // Assert
            var updatedUser = await _context.SystemUsers.FindAsync(1);
            updatedUser.Should().NotBeNull();
            updatedUser!.LastLoginAt.Should().NotBeNull();
            updatedUser.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        public async Task UpdateLastLoginAsync_NonExistingUser_DoesNothing()
        {
            // Arrange
            var user = new SystemUser
            {
                Id = 999,
                Username = "nonexistent",
                Password = "hashedpassword",
                IsActive = true
            };

            // Act & Assert
            await _repository.UpdateLastLoginAsync(user);
            // No exception should be thrown
        }

        [TestMethod]
        public async Task GetAllUsersAsync_WithMultipleUsers_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<SystemUser>
            {
                new SystemUser { Id = 1, Username = "user1", Password = "pass1", IsActive = true },
                new SystemUser { Id = 2, Username = "user2", Password = "pass2", IsActive = true },
                new SystemUser { Id = 3, Username = "user3", Password = "pass3", IsActive = true }
            };

            await _context.SystemUsers.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsersAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(u => u.Username == "user1");
            result.Should().Contain(u => u.Username == "user2");
            result.Should().Contain(u => u.Username == "user3");
        }

        [TestMethod]
        public async Task GetAllUsersAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllUsersAsync();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
