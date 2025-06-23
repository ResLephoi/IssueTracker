using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Infrastructure.Data;
using IssueTracker.Infrastructure.Repositories;
using IssueTracker.Domain.Entities;
using FluentAssertions;

namespace IssueTrackerTests.RepositoriesTests
{
    [TestClass]
    public class BoardRepositoryTests
    {
        private IssueTrackerDbContext _context = null!;
        private BoardRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<IssueTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new IssueTrackerDbContext(options);
            _repository = new BoardRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingBoard_ReturnsBoard()
        {
            // Arrange
            var board = new Board
            {
                Id = 1,
                Title = "Test Board"
            };
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Title.Should().Be("Test Board");
        }

        [TestMethod]
        public async Task GetByIdAsync_NonExistingBoard_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetBoardWithItemsAndCardsAsync_BoardWithItemsAndCards_ReturnsFullHierarchy()
        {
            // Arrange
            var board = new Board
            {
                Id = 1,
                Title = "Board with Items",
                Items = new List<Item>
                {
                    new Item
                    {
                        Id = 1,
                        Title = "Item 1",
                        BoardId = 1,
                        Cards = new List<Card>
                        {
                            new Card { Id = 1, Title = "Card 1", Description = "Desc 1", ItemId = 1 },
                            new Card { Id = 2, Title = "Card 2", Description = "Desc 2", ItemId = 1 }
                        }
                    },
                    new Item
                    {
                        Id = 2,
                        Title = "Item 2",
                        BoardId = 1,
                        Cards = new List<Card>
                        {
                            new Card { Id = 3, Title = "Card 3", Description = "Desc 3", ItemId = 2 }
                        }
                    }
                }
            };

            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetBoardWithItemsAndCardsAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Items.Should().HaveCount(2);
            result.Items!.First().Cards.Should().HaveCount(2);
            result.Items!.Skip(1).First().Cards.Should().HaveCount(1);
        }

        [TestMethod]
        public async Task GetBoardWithItemsAndCardsAsync_NonExistingBoard_ReturnsNull()
        {
            // Act
            var result = await _repository.GetBoardWithItemsAndCardsAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllAsync_WithMultipleBoards_ReturnsAllBoards()
        {
            // Arrange
            var boards = new List<Board>
            {
                new Board { Id = 1, Title = "Board 1" },
                new Board { Id = 2, Title = "Board 2" },
                new Board { Id = 3, Title = "Board 3" }
            };

            await _context.Boards.AddRangeAsync(boards);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(b => b.Title == "Board 1");
            result.Should().Contain(b => b.Title == "Board 2");
            result.Should().Contain(b => b.Title == "Board 3");
        }

        [TestMethod]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task AddAsync_ValidBoard_AddsBoardToDatabase()
        {
            // Arrange
            var board = new Board
            {
                Title = "New Board"
            };

            // Act
            await _repository.AddAsync(board);

            // Assert
            var savedBoard = await _context.Boards.FirstOrDefaultAsync();
            savedBoard.Should().NotBeNull();
            savedBoard!.Title.Should().Be("New Board");
        }

        [TestMethod]
        public async Task UpdateAsync_ExistingBoard_UpdatesBoard()
        {
            // Arrange
            var board = new Board
            {
                Id = 1,
                Title = "Original Title"
            };
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            // Detach the entity to simulate a fresh update scenario
            _context.Entry(board).State = EntityState.Detached;

            var updatedBoard = new Board
            {
                Id = 1,
                Title = "Updated Title"
            };

            // Act
            await _repository.UpdateAsync(updatedBoard);

            // Assert
            var result = await _context.Boards.FindAsync(1);
            result.Should().NotBeNull();
            result!.Title.Should().Be("Updated Title");
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingBoard_RemovesBoard()
        {
            // Arrange
            var board = new Board
            {
                Id = 1,
                Title = "Board to Delete"
            };
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(1);

            // Assert
            var deletedBoard = await _context.Boards.FindAsync(1);
            deletedBoard.Should().BeNull();
        }

        [TestMethod]
        public async Task DeleteAsync_NonExistingBoard_DoesNotThrowException()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _repository.DeleteAsync(999))
                .Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task GetBoardWithItemsAndCardsAsync_BoardWithNoItems_ReturnsBoardWithEmptyItems()
        {
            // Arrange
            var board = new Board
            {
                Id = 1,
                Title = "Empty Board"
            };
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetBoardWithItemsAndCardsAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Items.Should().BeNullOrEmpty();
        }
    }
}
