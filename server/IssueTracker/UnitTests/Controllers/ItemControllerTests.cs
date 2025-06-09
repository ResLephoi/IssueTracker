using AutoMapper;
using Domain.DTOs;
using IssueTracker.Application.Services;
using IssueTracker.Controllers;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
    [TestClass]
    public class ItemControllerTests
    {
        private Mock<IItemRepository> _mockItemRepository;
        private Mock<IMapper> _mockMapper;
        private ItemService _itemService;
        private ItemController _itemController;

        [TestInitialize]
        public void Setup()
        {
            // Set up mocks
            _mockItemRepository = new Mock<IItemRepository>();
            _mockMapper = new Mock<IMapper>();
            
            // Create service with mocked dependencies
            _itemService = new ItemService(_mockItemRepository.Object, _mockMapper.Object);
            
            // Create controller with mocked service
            _itemController = new ItemController(_itemService);
        }

        [TestMethod]
        public async Task GetById_ExistingId_ReturnsOkWithItem()
        {
            // Arrange
            var itemId = 1;
            var expectedItem = new Item { Id = itemId, Title = "Test Item", BoardId = 1 };
            
            _mockItemRepository.Setup(repo => repo.GetByIdAsync(itemId))
                .ReturnsAsync(expectedItem);

            // Act
            var result = await _itemController.GetById(itemId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            
            var returnedItem = okResult.Value as Item;
            Assert.IsNotNull(returnedItem);
            Assert.AreEqual(expectedItem.Id, returnedItem.Id);
            Assert.AreEqual(expectedItem.Title, returnedItem.Title);
        }

        [TestMethod]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingId = 999;
            
            _mockItemRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
                .ReturnsAsync((Item)null);

            // Act
            var result = await _itemController.GetById(nonExistingId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetAll_ReturnsOkWithAllItems()
        {
            // Arrange
            var expectedItems = new List<Item>
            {
                new Item { Id = 1, Title = "Item 1", BoardId = 1 },
                new Item { Id = 2, Title = "Item 2", BoardId = 1 },
                new Item { Id = 3, Title = "Item 3", BoardId = 2 }
            };
            
            _mockItemRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedItems);

            // Act
            var result = await _itemController.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            
            var returnedItems = okResult.Value as IEnumerable<Item>;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(expectedItems.Count, returnedItems.Count());
        }

        [TestMethod]
        public async Task Create_ValidItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var createItemDTO = new CreateItemDTO { Title = "New Item", BoardId = 1 };
            var createdItem = new Item { Id = 1, Title = "New Item", BoardId = 1 };
            
            _mockMapper.Setup(m => m.Map<Item>(createItemDTO))
                .Returns(createdItem);
                
            _mockItemRepository.Setup(repo => repo.AddAsync(createdItem))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _itemController.Create(createItemDTO);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(nameof(ItemController.GetById), createdAtActionResult.ActionName);
            
            var routeValues = createdAtActionResult.RouteValues;
            Assert.IsNotNull(routeValues);
            Assert.AreEqual(createdItem.Id, routeValues["id"]);
            
            var returnedItem = createdAtActionResult.Value as Item;
            Assert.IsNotNull(returnedItem);
            Assert.AreEqual(createdItem.Id, returnedItem.Id);
            Assert.AreEqual(createdItem.Title, returnedItem.Title);
        }

        [TestMethod]
        public async Task Update_ValidIdAndItem_ReturnsNoContent()
        {
            // Arrange
            var itemId = 1;
            var itemToUpdate = new Item { Id = itemId, Title = "Updated Item", BoardId = 1 };
            
            _mockItemRepository.Setup(repo => repo.UpdateAsync(itemToUpdate))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _itemController.Update(itemId, itemToUpdate);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task Update_MismatchedIds_ReturnsBadRequest()
        {
            // Arrange
            var itemId = 1;
            var mismatchedId = 2;
            var itemToUpdate = new Item { Id = mismatchedId, Title = "Updated Item", BoardId = 1 };

            // Act
            var result = await _itemController.Update(itemId, itemToUpdate);

            // Assert
            var badRequestResult = result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task Delete_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var itemId = 1;
            
            _mockItemRepository.Setup(repo => repo.DeleteAsync(itemId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _itemController.Delete(itemId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }
    }
}
