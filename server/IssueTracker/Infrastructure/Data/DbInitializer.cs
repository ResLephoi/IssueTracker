using System;
using System.Linq;
using IssueTracker.Application.Services;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void SeedUsers(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IssueTrackerDbContext>();
                  // Only seed if there are no users yet
                if (!dbContext.SystemUsers.Any())
                {
                    Console.WriteLine("Seeding sys users...");
                    
                    // Create admin user
                    dbContext.SystemUsers.Add(new SystemUser
                    {
                        Username = "admin",
                        Password = PasswordHasher.HashPassword("admin123"),
                        IsActive = true,
                        LastLoginAt = null
                    });
                    
                    // Create regular user
                    dbContext.SystemUsers.Add(new SystemUser
                    {
                        Username = "user",
                        Password = PasswordHasher.HashPassword("user123"),
                        IsActive = true,
                        LastLoginAt = null
                    });
                    
                    dbContext.SaveChanges();
                    Console.WriteLine("Users seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Users already exist, skipping seed.");
                }
            }
        }
    }
}
