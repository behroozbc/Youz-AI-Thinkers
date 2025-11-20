using Microsoft.EntityFrameworkCore;
using Youz_Backend.DB.Config;
using Youz_Backend.DB.Models;
namespace Youz_Backend.DB;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Landmark> Landmarks { get; set; }
    public DbSet<DescriptionChunk> DescriptionChunks { get; set; }
    public DbSet<ImageChunk> ImageChunks { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LandmarkConfig).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}