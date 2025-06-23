using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using AutoMapper;
using IssueTracker.Application.Services;
using IssueTracker.Domain.Interfaces;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.DTOs;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IssueTrackerTests.ServicesTests
{
    [TestClass]
    public class CardServiceTests
    {
        private Mock<ICardRepository> _mockCardRepository = null!;
        private Mock<IMapper> _mockMapper = null!;
        private CardService _cardService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockCardRepository = new Mock<ICardRepository>();
            _mockMapper = new Mock<IMapper>();
            _cardService = new CardService(_mockCardRepository.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task GetCardByIdAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockCardRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _cardService.GetCardByIdAsync(1));
        }

        [TestMethod]
        public async Task CreateCardAsync_NullInput_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _cardService.AddCardAsync(null!));
        }
                
        [TestMethod]
        public async Task CreateCardAsync_InvalidItemId_ThrowsValidationException()
        {
            // Arrange
            var createCardDTO = new CreateCardDTO { Title = "Test Card", ItemId = 0 };
            
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ValidationException>(() => 
                _cardService.AddCardAsync(createCardDTO));
        }
        
        [TestMethod]
        public async Task CreateCardAsync_InvalidAssignedUserId_HandledGracefully()
        {
            // Arrange
            var createCardDTO = new CreateCardDTO { 
                Title = "Test Card", 
                ItemId = 1, 
                AssignedToUserId = -1 
            };
            
            var mappedCard = new Card { 
                Title = "Test Card", 
                ItemId = 1, 
                AssignedToUserId = -1 
            };
            
            _mockMapper.Setup(m => m.Map<Card>(createCardDTO))
                .Returns(mappedCard);
                
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ValidationException>(() => 
                _cardService.AddCardAsync(createCardDTO));
        }

        [TestMethod]
        public async Task GetAllCardsAsync_EmptyList_ReturnsEmptyCollection()
        {
            // Arrange
            _mockCardRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Card>());
                
            // Act
            var result = await _cardService.GetAllCardsAsync();
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
