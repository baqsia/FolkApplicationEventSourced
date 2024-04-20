using ValueOf;

namespace FolkApplication.Domain.Song.ValueObjects;

public class SingerId : ValueOf<Guid, SingerId>
{
    public static SingerId New() => From(Guid.NewGuid());

    public static implicit operator Guid(SingerId songId) => songId.Value;

    public static implicit operator SingerId(Guid songId) => From(songId);
}