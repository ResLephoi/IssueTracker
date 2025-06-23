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
    public class BoardServiceTests
    {
        private Mock<IBoardRepository> _mockBoardRepository = null!;
        private Mock<IMapper> _mockMapper = null!;
        private BoardService _boardService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBoardRepository = new Mock<IBoardRepository>();
            _mockMapper = new Mock<IMapper>();
            _boardService = new BoardService(_mockBoardRepository.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task GetBoardByIdAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockBoardRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _boardService.GetBoardByIdAsync(1));
        }

        [TestMethod]
        public async Task CreateBoardAsync_NullInput_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _boardService.AddBoardAsync(null!));
        }

        [TestMethod]
        public async Task GetAllBoardsAsync_EmptyList_ReturnsEmptyCollection()
        {
            // Arrange
            _mockBoardRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Board>());
                
            // Act
            var result = await _boardService.GetAllBoardsAsync();
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public async Task UpdateBoardAsync_ConcurrencyConflict_ThrowsException()
        {
            // Arrange
            var boardId = 1;
            var board = new Board { Id = boardId, Title = "Board Title" };
            
            _mockBoardRepository.Setup(repo => repo.GetByIdAsync(boardId))
                .ReturnsAsync(board);
                
            _mockBoardRepository.Setup(repo => repo.UpdateAsync(board))
                .ThrowsAsync(new InvalidOperationException("Concurrency conflict detected"));
                
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => 
                _boardService.UpdateBoardAsync(board));
        }
    }
}
