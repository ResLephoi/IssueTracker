using AutoMapper;
using Domain.DTOs;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Interfaces;

namespace IssueTracker.Application.Services
{
    public class BoardService
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IMapper _mapper;

        public BoardService(IBoardRepository boardRepository, IMapper mapper)
        {
            _boardRepository = boardRepository;
            _mapper = mapper;
        }

        public async Task<Board> GetBoardByIdAsync(int id)
        {
            return await _boardRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync()
        {
            return await _boardRepository.GetAllAsync();
        }

        public async Task<Board> AddBoardAsync(CreateBoardDTO boardDto)
        {
            var board = _mapper.Map<Board>(boardDto);
            await _boardRepository.AddAsync(board);
            return board;
        }

        public async Task UpdateBoardAsync(Board board)
        {
            await _boardRepository.UpdateAsync(board);
        }

        public async Task DeleteBoardAsync(int id)
        {
            await _boardRepository.DeleteAsync(id);
        }
    }
}
