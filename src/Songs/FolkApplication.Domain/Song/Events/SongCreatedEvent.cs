using FolkApplication.Framework;

namespace FolkApplication.Domain.Song.Events;

public record SongCreatedEvent(Guid SongId, string Name, Singer[] Singers, long Version): DomainEvent;