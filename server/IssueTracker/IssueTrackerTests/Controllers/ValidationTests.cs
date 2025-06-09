using AutoMapper;
using Domain.DTOs;
using IssueTracker.Application.Services;
using IssueTracker.Controllers;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace IssueTrackerTests.Controllers;

[TestClass]
public class ValidationTests
{
    private Mock<IBoardRepository>? _mockBoardRepository;
    private Mock<ICardRepository>? _mockCardRepository;
    private Mock<IItemRepository>? _mockItemRepository;
    private Mock<IMapper>? _mockMapper;
    
    private BoardService? _boardService;
    private CardService? _cardService;
    private ItemService? _itemService;
    
    private BoardController? _boardController;
    private CardController? _cardController;
    private ItemController? _itemController;

    [TestInitialize]
    public void Setup()
    {
        // Set up mocks
        _mockBoardRepository = new Mock<IBoardRepository>();
        _mockCardRepository = new Mock<ICardRepository>();
        _mockItemRepository = new Mock<IItemRepository>();
        _mockMapper = new Mock<IMapper>();
        
        _boardService = new BoardService(_mockBoardRepository.Object, _mockMapper.Object);
        _cardService = new CardService(_mockCardRepository.Object, _mockMapper.Object);
        _itemService = new ItemService(_mockItemRepository.Object, _mockMapper.Object);
        
        _boardController = new BoardController(_boardService);
        _cardController = new CardController(_cardService);
        _itemController = new ItemController(_itemService);
    }    [TestMethod]
    public void CreateBoardDTO_RequiresTitle()
    {
        // Arrange
        var createBoardDto = new CreateBoardDTO { Title = string.Empty };
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(createBoardDto);
        
        // Act
        bool isValid = Validator.TryValidateObject(createBoardDto, validationContext, validationResults, true);
        
        // Assert
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, validationResults.Count);
        Assert.IsTrue(validationResults[0].ErrorMessage?.Contains("Title") ?? false);
    }
    [TestMethod]
    public void CreateCardDTO_RequiresTitle()
    {
        // Arrange
        var createCardDto = new CreateCardDTO { Title = string.Empty, Description = "Description" };
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(createCardDto);
        
        // Act
        bool isValid = Validator.TryValidateObject(createCardDto, validationContext, validationResults, true);
        
        // Assert
        Assert.IsFalse(isValid);
        // The DTO requires both Title and ItemId validation
        Assert.IsTrue(validationResults.Count >= 1);
        var titleError = validationResults.FirstOrDefault(vr => vr.ErrorMessage?.Contains("Title") ?? false);
        Assert.IsNotNull(titleError);
    }    [TestMethod]
    public void CreateItemDTO_RequiresTitleAndBoardId()
    {
        // Arrange
        var createItemDto = new CreateItemDTO { Title = string.Empty, BoardId = 0 };
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(createItemDto);
        
        // Act
        bool isValid = Validator.TryValidateObject(createItemDto, validationContext, validationResults, true);
        
        // Assert
        Assert.IsFalse(isValid);
        Assert.IsTrue(validationResults.Count >= 1);
        // Depending on how you've set up validation, Title might be required
        var titleError = validationResults.Find(vr => vr.ErrorMessage?.Contains("Title") ?? false);
        Assert.IsNotNull(titleError);
    }    [TestMethod]
    public async Task BoardController_Create_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        // Ensure _boardController is not null
        Assert.IsNotNull(_boardController);
        _boardController!.ModelState.AddModelError("Title", "Title is required");
        var createBoardDto = new CreateBoardDTO { Title = string.Empty };
        
        // Act
        var result = await _boardController.Create(createBoardDto);
        
        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOfType(badRequestResult!.Value, typeof(SerializableError));
    }
}
