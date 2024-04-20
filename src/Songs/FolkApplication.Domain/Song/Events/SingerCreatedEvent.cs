using FolkApplication.Domain.Song.ValueObjects;
using FolkApplication.Framework;

namespace FolkApplication.Domain.Song.Events;

public record SingerCreatedEvent(SingerId Id, string Name, bool IsBand, Song[] Songs, long Version) : DomainEvent
{
    
}