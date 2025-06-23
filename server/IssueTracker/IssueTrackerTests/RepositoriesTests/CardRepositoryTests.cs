using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Infrastructure.Data;
using IssueTracker.Repositories;
using IssueTracker.Domain.Entities;
using FluentAssertions;

namespace IssueTrackerTests.RepositoriesTests
{    [TestClass]
    public class CardRepositoryTests
    {
        private IssueTrackerDbContext _context = null!;
        private CardRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<IssueTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new IssueTrackerDbContext(options);
            _repository = new CardRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingCard_ReturnsCard()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Title = "Test Card",
                Description = "Test Description",
                ItemId = 1,
                Labels = new List<string> { "bug", "urgent" },
                AssignedToUserId = 1
            };
            await _context.Set<Card>().AddAsync(card);
            await _context.SaveChangesAsync();            
            
            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Title.Should().Be("Test Card");
            result.Description.Should().Be("Test Description");
            result.ItemId.Should().Be(1);
            result.AssignedToUserId.Should().Be(1);
        }

        [TestMethod]
        public async Task GetByIdAsync_NonExistingCard_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllAsync_WithMultipleCards_ReturnsAllCards()
        {
            // Arrange
            var cards = new List<Card>
            {
                new Card { Id = 1, Title = "Card 1", Description = "Desc 1", ItemId = 1 },
                new Card { Id = 2, Title = "Card 2", Description = "Desc 2", ItemId = 1 },
                new Card { Id = 3, Title = "Card 3", Description = "Desc 3", ItemId = 2 }
            };

            await _context.Set<Card>().AddRangeAsync(cards);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(c => c.Title == "Card 1");
            result.Should().Contain(c => c.Title == "Card 2");
            result.Should().Contain(c => c.Title == "Card 3");
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
        public async Task AddAsync_ValidCard_AddsCardToDatabase()
        {
            // Arrange
            var card = new Card
            {
                Title = "New Card",
                Description = "New Description",
                ItemId = 1,
                Labels = new List<string> { "feature" },
                AssignedToUserId = null
            };

            // Act
            await _repository.AddAsync(card);            
            
            // Assert
            var savedCard = await _context.Set<Card>().FirstOrDefaultAsync();
            savedCard.Should().NotBeNull();
            savedCard!.Title.Should().Be("New Card");
            savedCard.Description.Should().Be("New Description");
            savedCard.ItemId.Should().Be(1);
            savedCard.AssignedToUserId.Should().BeNull();
        }

        [TestMethod]
        public async Task UpdateAsync_ExistingCard_UpdatesCard()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Title = "Original Title",
                Description = "Original Description",
                ItemId = 1,
                Labels = new List<string> { "bug" }
            };
            await _context.Set<Card>().AddAsync(card);
            await _context.SaveChangesAsync();

            _context.Entry(card).State = EntityState.Detached;

            var updatedCard = new Card
            {
                Id = 1,
                Title = "Updated Title",
                Description = "Updated Description",
                ItemId = 1,
                Labels = new List<string> { "enhancement" },
                AssignedToUserId = 2
            };

            // Act
            await _repository.UpdateAsync(updatedCard);            
            
            // Assert
            var result = await _context.Set<Card>().FindAsync(1);
            result.Should().NotBeNull();
            result!.Title.Should().Be("Updated Title");
            result.Description.Should().Be("Updated Description");
            result.AssignedToUserId.Should().Be(2);
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingCard_RemovesCard()
        {
            // Arrange
            var card = new Card
            {
                Id = 1,
                Title = "Card to Delete",
                Description = "Description",
                ItemId = 1
            };
            await _context.Set<Card>().AddAsync(card);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(1);

            // Assert
            var deletedCard = await _context.Set<Card>().FindAsync(1);
            deletedCard.Should().BeNull();
        }

        [TestMethod]
        public async Task DeleteAsync_NonExistingCard_DoesNotThrowException()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _repository.DeleteAsync(999))
                .Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task AddAsync_CardWithLabels_StoresLabelsCorrectly()
        {
            // Arrange
            var labels = new List<string> { "bug", "urgent", "backend" };
            var card = new Card
            {
                Title = "Card with Labels",
                Description = "Test labels storage",
                ItemId = 1,
                Labels = labels
            };

            // Act
            await _repository.AddAsync(card);            
            
            // Assert
            var savedCard = await _context.Set<Card>().FirstOrDefaultAsync();
            savedCard.Should().NotBeNull();
            savedCard!.Labels.Should().HaveCount(3);
            savedCard.Labels.Should().Contain("bug");
            savedCard.Labels.Should().Contain("urgent");
            savedCard.Labels.Should().Contain("backend");
        }
    }
}
