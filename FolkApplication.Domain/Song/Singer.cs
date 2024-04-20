using FolkApplication.Domain.Song.Events;
using FolkApplication.Domain.Song.ValueObjects;
using FolkApplication.Framework;

namespace FolkApplication.Domain.Song;

public record Singer : AggregateRoot
{
    public string Name { get; set; }
    public bool IsBand { get; set; }
    public Song[] Songs { get; set; } = [];


    public Singer()
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