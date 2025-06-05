using Domain.DTOs;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Interfaces;
using IssueTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Repositories
{
    public class CardRepository: ICardRepository
    {
        private readonly IssueTrackerDbContext _context;

        public CardRepository(IssueTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Card?> GetByIdAsync(int id)
        {
            return await _context.Set<Card>().FindAsync(id);
        }

        public async Task<IEnumerable<Card>> GetAllAsync()
        {
            return await _context.Set<Card>().ToListAsync();
        }

        public async Task AddAsync(Card card)
        {
            await _context.Set<Card>().AddAsync(card);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Card card)
        {
            _context.Set<Card>().Update(card);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var card = await GetByIdAsync(id);
            if (card != null)
            {
                _context.Set<Card>().Remove(card);
                await _context.SaveChangesAsync();
            }
        }
    }
}
