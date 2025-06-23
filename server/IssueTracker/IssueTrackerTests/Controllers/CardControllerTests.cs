using AutoMapper;
using IssueTracker.Domain.DTOs;
using IssueTracker.Application.Services;
using IssueTracker.Controllers;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueTrackerTests.Controllers;

[TestClass]
public class CardControllerTests
{
    private Mock<ICardRepository> _mockCardRepository;
    private Mock<IMapper> _mockMapper;
    private CardService _cardService;
    private CardController _cardController;

    [TestInitialize]
    public void Setup()
    {
        // Set up mocks
        _mockCardRepository = new Mock<ICardRepository>();
        _mockMapper = new Mock<IMapper>();
        _cardService = new CardService(_mockCardRepository.Object, _mockMapper.Object);
        _cardController = new CardController(_cardService);
    }

    [TestMethod]
    public async Task GetById_ExistingId_ReturnsOkWithCard()
    {
        // Arrange
        var cardId = 1;
        var expectedCard = new Card { Id = cardId, Title = "Test Card", Description = "Test Description" };
        
        _mockCardRepository.Setup(repo => repo.GetByIdAsync(cardId))
            .ReturnsAsync(expectedCard);

        // Act
        var result = await _cardController.GetById(cardId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        
        var returnedCard = okResult.Value as Card;
        Assert.IsNotNull(returnedCard);
        Assert.AreEqual(expectedCard.Id, returnedCard.Id);
        Assert.AreEqual(expectedCard.Title, returnedCard.Title);
    }

    [TestMethod]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = 999;
          _mockCardRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Card?)null);

        // Act
        var result = await _cardController.GetById(nonExistingId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task GetAll_ReturnsOkWithAllCards()
    {
        // Arrange
        var expectedCards = new List<Card>
        {
            new Card { Id = 1, Title = "Card 1", Description = "Description 1" },
            new Card { Id = 2, Title = "Card 2", Description = "Description 2" },
            new Card { Id = 3, Title = "Card 3", Description = "Description 3" }
        };
        
        _mockCardRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedCards);

        // Act
        var result = await _cardController.GetAll();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        
        var returnedCards = okResult.Value as IEnumerable<Card>;
        Assert.IsNotNull(returnedCards);
        Assert.AreEqual(expectedCards.Count, returnedCards.Count());
    }

    [TestMethod]
    public async Task Create_ValidCard_ReturnsCreatedAtAction()
    {
        // Arrange
        var createCardDTO = new CreateCardDTO { ItemId = 1, Title = "New Card", Description = "New Description" };
        var createdCard = new Card { Id = 1, Title = "New Card", Description = "New Description" };
        
        _mockMapper.Setup(m => m.Map<Card>(createCardDTO))
            .Returns(createdCard);
            
        _mockCardRepository.Setup(repo => repo.AddAsync(createdCard))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cardController.Create(createCardDTO);

        // Assert
        var createdAtActionResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdAtActionResult);
        Assert.AreEqual(201, createdAtActionResult.StatusCode);
        Assert.AreEqual(nameof(CardController.GetById), createdAtActionResult.ActionName);
        
        var routeValues = createdAtActionResult.RouteValues;
        Assert.IsNotNull(routeValues);
        Assert.AreEqual(createdCard.Id, routeValues["id"]);
        
        var returnedCard = createdAtActionResult.Value as Card;
        Assert.IsNotNull(returnedCard);
        Assert.AreEqual(createdCard.Id, returnedCard.Id);
        Assert.AreEqual(createdCard.Title, returnedCard.Title);
    }

    [TestMethod]
    public async Task Update_ValidCard_ReturnsNoContent()
    {
        // Arrange
        var updateCardDTO = new UpdateCardDTO { Id = 1, Title = "Updated Card", Description = "Updated Description" };
        var cardToUpdate = new Card { Id = 1, Title = "Updated Card", Description = "Updated Description" };
        
        _mockMapper.Setup(m => m.Map<Card>(updateCardDTO))
            .Returns(cardToUpdate);
            
        _mockCardRepository.Setup(repo => repo.UpdateAsync(cardToUpdate))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cardController.Update(updateCardDTO);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task Update_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var updateCardDTO = new UpdateCardDTO { Id = 0, Title = "Updated Card", Description = "Updated Description" };

        // Act
        var result = await _cardController.Update(updateCardDTO);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var cardId = 1;
        
        _mockCardRepository.Setup(repo => repo.DeleteAsync(cardId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cardController.Delete(cardId);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }
}
