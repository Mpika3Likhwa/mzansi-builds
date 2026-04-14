using mzansi_builds_api.Models;
using Microsoft.EntityFrameworkCore;

namespace mzansi_builds_api.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(AppDbContext context) // Ensure context name matches yours
        {
            // 1. Check if users already exist to avoid duplicates
            if (await context.Users.AnyAsync()) return;

            // 2. Create 3 Users (Passwords are just placeholders)
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();
            var user3Id = Guid.NewGuid();

            var users = new List<User>
            {
                new User { Id = user1Id, Username = "SiphoDev", Email = "sipho@mzansi.com", PasswordHash = "hashed_pw_1" },
                new User { Id = user2Id, Username = "LeilaCodes", Email = "leila@mzansi.com", PasswordHash = "hashed_pw_2" },
                new User { Id = user3Id, Username = "ChrisBuilds", Email = "chris@mzansi.com", PasswordHash = "hashed_pw_3" }
            };

            await context.Users.AddRangeAsync(users);

            // 3. Create Projects linked to these Users
            var projects = new List<Project>
            {
                new Project
                {
                    Title = "Mzansi Builders Portal",
                    Description = "A community hub for South African devs.",
                    UserId = user1Id.ToString(),
                    GitHubRepoUrl = "https://github.com/SiphoDev/mzansi-portal",
                    IsFullyCompleted = false,
                    Stages = new List<ProjectStage>
                    {
                        new ProjectStage { Name = "Database Design", IsCompleted = true },
                        new ProjectStage { Name = "API Integration", SupportRequired = "Need help with Azure Auth", IsCompleted = false }
                    }
                },
                new Project
                {
                    Title = "LoadShedding Tracker Pro",
                    Description = "Real-time Eskom data visualization.",
                    UserId = user2Id.ToString(),
                    GitHubRepoUrl = "https://github.com/LeilaCodes/ls-tracker",
                    IsFullyCompleted = true,
                    Stages = new List<ProjectStage>
                    {
                        new ProjectStage { Name = "Scraper Setup", IsCompleted = true },
                        new ProjectStage { Name = "Frontend Dashboard", IsCompleted = true }
                    }
                },
                new Project
                {
                    Title = "Taxi Route Optimizer",
                    Description = "ML tool for local commute efficiency.",
                    UserId = user3Id.ToString(),
                    GitHubRepoUrl = "https://github.com/ChrisBuilds/taxi-ai",
                    IsFullyCompleted = false,
                    Stages = new List<ProjectStage>
                    {
                        new ProjectStage { Name = "Data Collection", SupportRequired = "Need GPS datasets", IsCompleted = false }
                    }
                }
            };

            await context.Projects.AddRangeAsync(projects);
            await context.SaveChangesAsync(); // Save to get Project IDs for the requests

            // 4. Create Collaboration Requests
            var collabRequests = new List<CollaborationRequest>
            {
                new CollaborationRequest
                {
                    ProjectId = projects[0].Id,
                    RequesterUserId = user2Id.ToString(),
                    Message = "I have experience with Azure AD, would love to help!",
                    Status = CollabStatus.Pending
                },
                new CollaborationRequest
                {
                    ProjectId = projects[2].Id,
                    RequesterUserId = user1Id.ToString(),
                    Message = "Great idea! I can help with the backend.",
                    Status = CollabStatus.Accepted
                }
            };

            await context.CollaborationRequests.AddRangeAsync(collabRequests);
            await context.SaveChangesAsync();
        }
    }
}