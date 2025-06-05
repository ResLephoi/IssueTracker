using IssueTracker.Domain.Entities;

namespace IssueTracker.Domain.Interfaces
{
    public interface IBoardRepository
    {
        Task<Board> GetByIdAsync(int id);
        Task<Board> GetBoardWithItemsAndCardsAsync(int id);
        Task<IEnumerable<Board>> GetAllAsync();
        Task AddAsync(Board board);
        Task UpdateAsync(Board board);
        Task DeleteAsync(int id);
    }
}
