using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Interfaces;
using IssueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Infrastructure.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly IssueTrackerDbContext _context;

        public BoardRepository(IssueTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Board> GetByIdAsync(int id)
        {
            return await _context.Boards.FindAsync(id);
        }

        public async Task<Board> GetBoardWithItemsAndCardsAsync(int id)
        {
            return await _context.Boards
                .Include(b => b.Items)
                    .ThenInclude(i => i.Cards)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Board>> GetAllAsync()
        {
            return await _context.Boards.ToListAsync();
        }

        public async Task AddAsync(Board board)
        {
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Board board)
        {
            _context.Boards.Update(board);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var board = await GetByIdAsync(id);
            if (board != null)
            {
                _context.Boards.Remove(board);
                await _context.SaveChangesAsync();
            }
        }
    }
}
