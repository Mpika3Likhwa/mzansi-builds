using Microsoft.EntityFrameworkCore;
using mzansi_builds_api.Models;

namespace mzansi_builds_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
<<<<<<< HEAD
    //public DbSet<Project> Projects { get; set; }
=======
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectStage> ProjectStages { get; set; }
>>>>>>> de313b0a83413e0f894c94718f819616237932ae
}