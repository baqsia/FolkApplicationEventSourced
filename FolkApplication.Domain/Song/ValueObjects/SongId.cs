using ValueOf;

namespace FolkApplication.Domain.Song.ValueObjects;

public class SongId : ValueOf<Guid, SongId>
{
    public static SongId New() => From(Guid.NewGuid());

    public static implicit operator Guid(SongId songId) => songId.Value;
    
    public static implicit  operator SongId(Guid songId) => From(songId);
}