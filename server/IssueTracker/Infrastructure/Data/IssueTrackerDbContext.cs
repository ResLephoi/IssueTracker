using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Infrastructure.Data
{
    public class IssueTrackerDbContext : DbContext
    {
        public IssueTrackerDbContext(DbContextOptions<IssueTrackerDbContext> options) : base(options) { }

        public DbSet<Board> Boards { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Board>()
                .HasMany(b => b.Items)
                .WithOne(i => i.Board)
                .HasForeignKey(i => i.BoardId);

            modelBuilder.Entity<Item>()
                .HasMany(i => i.Cards)
                .WithOne(c => c.Item)
                .HasForeignKey(c => c.ItemId);
        }
    }
}
