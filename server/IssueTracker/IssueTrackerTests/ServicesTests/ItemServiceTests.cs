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
    public class ItemServiceTests
    {
        private Mock<IItemRepository> _mockItemRepository = null!;
        private Mock<IMapper> _mockMapper = null!;
        private ItemService _itemService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockItemRepository = new Mock<IItemRepository>();
            _mockMapper = new Mock<IMapper>();
            _itemService = new ItemService(_mockItemRepository.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task GetItemByIdAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockItemRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _itemService.GetItemByIdAsync(1));
        }

        [TestMethod]
        public async Task CreateItemAsync_NullInput_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _itemService.AddItemAsync(null!));
        }

        [TestMethod]
        public async Task CreateItemAsync_EmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var createItemDTO = new CreateItemDTO { Title = "", BoardId = 1 };
            
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ValidationException>(() => _itemService.AddItemAsync(createItemDTO));
        }
        
        [TestMethod]
        public async Task CreateItemAsync_InvalidBoardId_ThrowsArgumentException()
        {
            // Arrange
            var createItemDTO = new CreateItemDTO { Title = "Test Item", BoardId = 0 };
            
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ValidationException>(() => 
                _itemService.AddItemAsync(createItemDTO));
        }
                
        [TestMethod]
        public async Task GetAllItemsAsync_EmptyList_ReturnsEmptyCollection()
        {
            // Arrange
            _mockItemRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Item>());
                
            // Act
            var result = await _itemService.GetAllItemsAsync();
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
                
        [TestMethod]
        public async Task UpdateItemAsync_ChangeBoardId_UpdatesRelationship()
        {
            // Arrange
            var itemId = 1;
            var originalItem = new Item { Id = itemId, Title = "Test Item", BoardId = 1 };
            var updatedItem = new Item { Id = itemId, Title = "Test Item Updated", BoardId = 2 };
            
            _mockItemRepository.Setup(repo => repo.GetByIdAsync(itemId))
                .ReturnsAsync(originalItem);
                
            // Act
            await _itemService.UpdateItemAsync(updatedItem);
            
            // Assert
            _mockItemRepository.Verify(repo => repo.UpdateAsync(updatedItem), Times.Once);
        }
    }
}
