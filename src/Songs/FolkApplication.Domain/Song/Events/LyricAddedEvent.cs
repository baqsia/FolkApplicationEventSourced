using FolkApplication.Framework;

namespace FolkApplication.Domain.Song.Events;

public record LyricAddedEvent(string Value, Guid SongId): DomainEvent;