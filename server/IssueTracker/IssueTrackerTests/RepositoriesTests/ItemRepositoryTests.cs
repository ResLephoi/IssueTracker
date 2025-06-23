using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Infrastructure.Data;
using IssueTracker.Infrastructure.Repositories;
using IssueTracker.Domain.Entities;
using FluentAssertions;

namespace IssueTrackerTests.RepositoriesTests
{    [TestClass]
    public class ItemRepositoryTests
    {
        private IssueTrackerDbContext _context = null!;
        private ItemRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<IssueTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new IssueTrackerDbContext(options);
            _repository = new ItemRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingItem_ReturnsItem()
        {
            // Arrange
            var item = new Item
            {
                Id = 1,
                Title = "Test Item",
                BoardId = 1
            };
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);            
            
            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Title.Should().Be("Test Item");
            result.BoardId.Should().Be(1);
        }

        [TestMethod]
        public async Task GetByIdAsync_NonExistingItem_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllAsync_WithMultipleItems_ReturnsAllItems()
        {
            // Arrange
            var items = new List<Item>
            {                new Item { Id = 1, Title = "Item 1", BoardId = 1 },
                new Item { Id = 2, Title = "Item 2", BoardId = 1 },
                new Item { Id = 3, Title = "Item 3", BoardId = 2 }
            };

            await _context.Items.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);            result.Should().Contain(i => i.Title == "Item 1");
            result.Should().Contain(i => i.Title == "Item 2");
            result.Should().Contain(i => i.Title == "Item 3");
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
        public async Task AddAsync_ValidItem_AddsItemToDatabase()
        {
            // Arrange
            var item = new Item
            {
                Title = "New Item",
                BoardId = 1
            };

            // Act
            await _repository.AddAsync(item);            
            
            // Assert
            var savedItem = await _context.Items.FirstOrDefaultAsync();
            savedItem.Should().NotBeNull();
            savedItem!.Title.Should().Be("New Item");
            savedItem.BoardId.Should().Be(1);
        }

        [TestMethod]
        public async Task UpdateAsync_ExistingItem_UpdatesItem()
        {
            // Arrange
            var item = new Item
            {
                Id = 1,
                Title = "Original Name",
                BoardId = 1
            };
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            _context.Entry(item).State = EntityState.Detached;

            var updatedItem = new Item
            {
                Id = 1,
                Title = "Updated Name",
                BoardId = 2
            };

            // Act
            await _repository.UpdateAsync(updatedItem);            
            
            // Assert
            var result = await _context.Items.FindAsync(1);
            result.Should().NotBeNull();
            result!.Title.Should().Be("Updated Name");
            result.BoardId.Should().Be(2);
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingItem_RemovesItem()
        {
            // Arrange
            var item = new Item
            {
                Id = 1,
                Title = "Item to Delete",
                BoardId = 1
            };
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(1);

            // Assert
            var deletedItem = await _context.Items.FindAsync(1);
            deletedItem.Should().BeNull();
        }

        [TestMethod]
        public async Task DeleteAsync_NonExistingItem_DoesNotThrowException()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _repository.DeleteAsync(999))
                .Should().NotThrowAsync();
        }

        [TestMethod]
        public async Task AddAsync_MultipleItemsForSameBoard_AllItemsAreStored()
        {
            // Arrange
            var items = new List<Item>
            {                new Item { Title = "Item 1", BoardId = 1 },
                new Item { Title = "Item 2", BoardId = 1 },
                new Item { Title = "Item 3", BoardId = 1 }
            };

            // Act
            foreach (var item in items)
            {
                await _repository.AddAsync(item);
            }

            // Assert
            var allItems = await _context.Items.Where(i => i.BoardId == 1).ToListAsync();
            allItems.Should().HaveCount(3);            allItems.Should().Contain(i => i.Title == "Item 1");
            allItems.Should().Contain(i => i.Title == "Item 2");
            allItems.Should().Contain(i => i.Title == "Item 3");
        }
    }
}
