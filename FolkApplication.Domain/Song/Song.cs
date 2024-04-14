using FolkApplication.Domain.Song.Events;
using FolkApplication.Domain.Song.ValueObjects;
using FolkApplication.Framework;

namespace FolkApplication.Domain.Song;

public class Song: AggregateRoot
{
    public string Name { get; set; } = default!;
    
    public Song()
    {
        
    }
    
    private Song(string name)
    {
        var id = SongId.New();
        Id = id;
        Event(new SongCreatedEvent(id, name));
    }

    public static Song Create(string name)
    {
        return new Song(name);
    }

    public void Apply(SongCreatedEvent @event)
    {
        Name = @event.Name;
        Id = @event.SongId;
    }
}