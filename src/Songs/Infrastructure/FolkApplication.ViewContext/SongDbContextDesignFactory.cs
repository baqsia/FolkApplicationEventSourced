using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FolkApplication.ViewContext;

public class SongDbContextDesignFactory : IDesignTimeDbContextFactory<SongDbContext>
{
    public SongDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SongDbContext>();
        const string connectionString = "Host=localhost;Port=5434;User ID=admin;Password=adminpass;Database=folk-songs;";

        builder.UseNpgsql(connectionString);
        return new SongDbContext(builder.Options);
    }
}