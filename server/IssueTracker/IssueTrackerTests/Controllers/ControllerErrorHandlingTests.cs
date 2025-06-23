using AutoMapper;
using IssueTracker.Domain.DTOs;
using IssueTracker.Application.Services;
using IssueTracker.Controllers;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;

namespace IssueTrackerTests.Controllers;

[TestClass]
public class ControllerErrorHandlingTests
{
    private Mock<IBoardRepository> _mockBoardRepository;
    private Mock<ICardRepository> _mockCardRepository;
    private Mock<IItemRepository> _mockItemRepository;
    private Mock<IMapper> _mockMapper;
    
    private BoardService _boardService;
    private CardService _cardService;
    private ItemService _itemService;
    
    private BoardController _boardController;
    private CardController _cardController;
    private ItemController _itemController;

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
    }

    [TestMethod]
    public async Task BoardController_GetById_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        var boardId = 1;
        _mockBoardRepository.Setup(repo => repo.GetByIdAsync(boardId))
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await _boardController.GetById(boardId);

        // Assert
        var statusCodeResult = result as StatusCodeResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }

    [TestMethod]
    public async Task CardController_GetAll_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        _mockCardRepository.Setup(repo => repo.GetAllAsync())
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await _cardController.GetAll();

        // Assert
        var statusCodeResult = result as StatusCodeResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }

    [TestMethod]
    public async Task ItemController_Create_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        var createItemDto = new CreateItemDTO { Title = "Test Item", BoardId = 1 };
        var item = new Item { Id = 1, Title = "Test Item", BoardId = 1 };
        
        _mockMapper.Setup(m => m.Map<Item>(createItemDto))
            .Returns(item);
            
        _mockItemRepository.Setup(repo => repo.AddAsync(item))
            .ThrowsAsync(new Exception("Database connection error"));

        // Act
        var result = await _itemController.Create(createItemDto);

        // Assert
        var statusCodeResult = result as StatusCodeResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
}
