using FolkApplication.ViewContext.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace FolkApplication.ViewContext;

public class SongDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("songs_flat");
        modelBuilder.ApplyConfiguration(new SongEntityConfiguration());
    }
}