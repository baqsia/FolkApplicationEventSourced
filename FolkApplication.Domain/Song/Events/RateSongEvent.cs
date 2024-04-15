using FolkApplication.Domain.Song.ValueObjects;
using FolkApplication.Framework;

namespace FolkApplication.Domain.Song.Events;

public record RateSongEvent(SongId SongId, int Rating): DomainEvent;