using FolkApplication.Domain.Song.Events;
using FolkApplication.Domain.Song.ValueObjects;
using FolkApplication.Framework;

namespace FolkApplication.Domain.Song;

public record Song: AggregateRoot
{
    public string Name { get; set; } = default!;

    public int Rating { get; set; }
    
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

    public void Rate(int rating)
    {
        Event(new RateSongEvent(Id, rating));
    }
    
    
    public void Apply(SongCreatedEvent @event)
    {
        Name = @event.Name;
        Id = @event.SongId;
    }

    public void Apply(RateSongEvent @event)
    {
        Rating = @event.Rating;
    }
}