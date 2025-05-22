using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IssueTracker.Infrastructure.Data
{
    public class IssueTrackerDbContextFactory : IDesignTimeDbContextFactory<IssueTrackerDbContext>
    {
        public IssueTrackerDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "IssueTracker");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<IssueTrackerDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new IssueTrackerDbContext(optionsBuilder.Options);
        }
    }
}