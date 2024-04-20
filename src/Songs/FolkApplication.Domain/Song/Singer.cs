using FolkApplication.Domain.Song.Events;
using FolkApplication.Domain.Song.ValueObjects;
using FolkApplication.Framework;

namespace FolkApplication.Domain.Song;

public record Singer : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public bool IsBand { get; private set; }
    public Song[] Songs { get; private set; } = [];


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Singer()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    private Singer(string name, bool isBand, params Song[] songs)
    {
        var id = SingerId.New();
        Id = id;
        var @event = new SingerCreatedEvent(id, name, isBand, songs, 1);
        Event(@event);
        Apply(@event);
    }

    public static Singer New(string name, bool isBand = false, params Song[] songs)
    {
        return new Singer(name, isBand, songs);
    }

    public void Apply(SingerCreatedEvent @event)
    {
        Name = @event.Name;
        Version = @event.Version;
        Songs = @event.Songs;
        IsBand = @event.IsBand;
    }
}