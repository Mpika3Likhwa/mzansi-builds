using Microsoft.EntityFrameworkCore;
using mzansi_builds_api.Models;

namespace mzansi_builds_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectStage> ProjectStages { get; set; }
}