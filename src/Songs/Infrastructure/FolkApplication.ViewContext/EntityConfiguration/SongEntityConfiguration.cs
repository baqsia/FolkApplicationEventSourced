using FolkApplication.Domain.Song;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FolkApplication.ViewContext.EntityConfiguration;

public class SongEntityConfiguration: IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.ToTable("song");
    }
}