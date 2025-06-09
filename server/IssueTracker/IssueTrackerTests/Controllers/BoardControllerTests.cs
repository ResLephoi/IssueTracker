using AutoMapper;
using Domain.DTOs;
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
public class BoardControllerTests
{
    private Mock<IBoardRepository>? _mockBoardRepository;
    private Mock<IMapper>? _mockMapper;
    private BoardService? _boardService;
    private BoardController? _boardController;

    [TestInitialize]
    public void Setup()
    {
        // Set up mocks
        _mockBoardRepository = new Mock<IBoardRepository>();
        _mockMapper = new Mock<IMapper>();        
        _boardService = new BoardService(_mockBoardRepository.Object, _mockMapper.Object);        
        _boardController = new BoardController(_boardService);
    }

    [TestMethod]
    public async Task GetById_ExistingId_ReturnsOkWithBoard()
    {
        // Arrange
        var boardId = 1;
        var expectedBoard = new Board { Id = boardId, Title = "Test Board" };
        
        _mockBoardRepository.Setup(repo => repo.GetByIdAsync(boardId))
            .ReturnsAsync(expectedBoard);

        // Act
        var result = await _boardController.GetById(boardId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        
        var returnedBoard = okResult.Value as Board;
        Assert.IsNotNull(returnedBoard);
        Assert.AreEqual(expectedBoard.Id, returnedBoard.Id);
        Assert.AreEqual(expectedBoard.Title, returnedBoard.Title);
    }

    [TestMethod]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = 999;
          _mockBoardRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Board?)null);

        // Act
        var result = await _boardController.GetById(nonExistingId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task GetBoardWithItemsAndCards_ExistingId_ReturnsOkWithFullBoard()
    {
        // Arrange
        var boardId = 1;
        var items = new List<Item> 
        {
            new Item { Id = 1, Title = "Item 1", BoardId = boardId },
            new Item { Id = 2, Title = "Item 2", BoardId = boardId }
        };
        
        var expectedBoard = new Board 
        { 
            Id = boardId, 
            Title = "Test Board", 
            Items = items 
        };
        
        _mockBoardRepository.Setup(repo => repo.GetBoardWithItemsAndCardsAsync(boardId))
            .ReturnsAsync(expectedBoard);

        // Act
        var result = await _boardController.GetBoardWithItemsAndCards(boardId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        
        var returnedBoard = okResult.Value as Board;
        Assert.IsNotNull(returnedBoard);
        Assert.AreEqual(expectedBoard.Id, returnedBoard.Id);
        Assert.AreEqual(expectedBoard.Title, returnedBoard.Title);
        Assert.AreEqual(expectedBoard.Items.Count, returnedBoard.Items.Count);
    }

    [TestMethod]
    public async Task GetBoardWithItemsAndCards_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = 999;
          _mockBoardRepository.Setup(repo => repo.GetBoardWithItemsAndCardsAsync(nonExistingId))
            .ReturnsAsync((Board?)null);

        // Act
        var result = await _boardController.GetBoardWithItemsAndCards(nonExistingId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task GetAll_ReturnsOkWithAllBoards()
    {
        // Arrange
        var expectedBoards = new List<Board>
        {
            new Board { Id = 1, Title = "Board 1" },
            new Board { Id = 2, Title = "Board 2" },
            new Board { Id = 3, Title = "Board 3" }
        };
        
        _mockBoardRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedBoards);

        // Act
        var result = await _boardController.GetAll();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        
        var returnedBoards = okResult.Value as IEnumerable<Board>;
        Assert.IsNotNull(returnedBoards);
        Assert.AreEqual(expectedBoards.Count, returnedBoards.Count());
    }

    [TestMethod]
    public async Task Create_ValidBoard_ReturnsCreatedAtAction()
    {
        // Arrange
        var createBoardDto = new CreateBoardDTO { Title = "New Board" };
        var createdBoard = new Board { Id = 1, Title = "New Board" };
        
        _mockMapper.Setup(m => m.Map<Board>(createBoardDto))
            .Returns(createdBoard);
            
        _mockBoardRepository.Setup(repo => repo.AddAsync(createdBoard))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _boardController.Create(createBoardDto);

        // Assert
        var createdAtActionResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdAtActionResult);
        Assert.AreEqual(201, createdAtActionResult.StatusCode);
        Assert.AreEqual(nameof(BoardController.GetById), createdAtActionResult.ActionName);
        
        var routeValues = createdAtActionResult.RouteValues;
        Assert.IsNotNull(routeValues);
        Assert.AreEqual(createdBoard.Id, routeValues["id"]);
        
        var returnedBoard = createdAtActionResult.Value as Board;
        Assert.IsNotNull(returnedBoard);
        Assert.AreEqual(createdBoard.Id, returnedBoard.Id);
        Assert.AreEqual(createdBoard.Title, returnedBoard.Title);
    }

    [TestMethod]
    public async Task Update_ValidIdAndBoard_ReturnsNoContent()
    {
        // Arrange
        var boardId = 1;
        var boardToUpdate = new Board { Id = boardId, Title = "Updated Board" };
        
        _mockBoardRepository.Setup(repo => repo.UpdateAsync(boardToUpdate))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _boardController.Update(boardId, boardToUpdate);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task Update_MismatchedIds_ReturnsBadRequest()
    {
        // Arrange
        var boardId = 1;
        var mismatchedId = 2;
        var boardToUpdate = new Board { Id = mismatchedId, Title = "Updated Board" };

        // Act
        var result = await _boardController.Update(boardId, boardToUpdate);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var boardId = 1;
        
        _mockBoardRepository.Setup(repo => repo.DeleteAsync(boardId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _boardController.Delete(boardId);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }
}
