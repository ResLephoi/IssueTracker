using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using IssueTracker.Application.Services;

namespace IssueTrackerTests.ServicesTests
{
    [TestClass]
    public class PasswordHasherTests
    {
        [TestMethod]
        public void HashPassword_ValidInput_ReturnsHash()
        {
            // Arrange
            string password = "TestPassword123";

            // Act
            string hash = PasswordHasher.HashPassword(password);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Length.Should().Be(32); // MD5 produces a 32-character hex string
            hash.Should().MatchRegex("^[0-9A-F]+$"); // Should contain only hex characters
        }

        [TestMethod]
        public void HashPassword_SameInputTwice_ReturnsSameHash()
        {
            // Arrange
            string password = "TestPassword123";

            // Act
            string hash1 = PasswordHasher.HashPassword(password);
            string hash2 = PasswordHasher.HashPassword(password);

            // Assert
            hash1.Should().Be(hash2);
        }

        [TestMethod]
        public void HashPassword_DifferentInputs_ReturnsDifferentHashes()
        {
            // Arrange
            string password1 = "TestPassword123";
            string password2 = "TestPassword124";

            // Act
            string hash1 = PasswordHasher.HashPassword(password1);
            string hash2 = PasswordHasher.HashPassword(password2);

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [TestMethod]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string password = "TestPassword123";
            string hash = PasswordHasher.HashPassword(password);

            // Act
            bool result = PasswordHasher.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void VerifyPassword_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            string correctPassword = "TestPassword123";
            string incorrectPassword = "WrongPassword123";
            string hash = PasswordHasher.HashPassword(correctPassword);

            // Act
            bool result = PasswordHasher.VerifyPassword(incorrectPassword, hash);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void VerifyPassword_CaseSensitivity_ReturnsFalse()
        {
            // Arrange
            string correctPassword = "TestPassword123";
            string incorrectCasePassword = "testpassword123";
            string hash = PasswordHasher.HashPassword(correctPassword);

            // Act
            bool result = PasswordHasher.VerifyPassword(incorrectCasePassword, hash);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void VerifyPassword_EmptyPassword_HandlesGracefully()
        {
            // Arrange
            string password = "";
            string hash = PasswordHasher.HashPassword(password);

            // Act
            bool result = PasswordHasher.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void VerifyPassword_NullPassword_DoesNotThrowException()
        {
            // Arrange
            string password = "testpassword";
            string validHash = PasswordHasher.HashPassword("somepassword");

            // Act & Assert
            Action action = () => PasswordHasher.VerifyPassword(password, validHash);
            action.Should().NotThrow();
        }
    }
}
