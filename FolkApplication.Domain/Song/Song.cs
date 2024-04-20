using FolkApplication.Domain.Song.Events;
using FolkApplication.Domain.Song.ValueObjects;
using FolkApplication.Framework;

namespace FolkApplication.Domain.Song;

public record Song: AggregateRoot
{
    public string Name { get; set; } = default!;
    public int Rating { get; set; }
    public Singer[] Singers { get; set; } = [];
    public List<Lyric> Lyrics { get; set; } = [];
    
    public Song()
    {
        
    }
    
    private Song(string name, Singer[] singers)
    {
        var id = SongId.New();
        Id = id;
        var @event = new SongCreatedEvent(id, name, singers, Version);
        Event(@event);
        Apply(@event);
    }

    public static Song New(string name, params Singer[] singers)
    {
        return new Song(name, singers);
    }

    public void Rate(int rating)
    {
        Event(new RateSongEvent(Id, rating)
        {
            Version = Version
        });
    }
    
    public void AddLyrics(string lyrics)
    {
        Event(new LyricAddedEvent(lyrics, Id)
        {
            Version = Version
        });
    }
    
    public void Apply(SongCreatedEvent @event)
    {
        Name = @event.Name;
        Id = @event.SongId;
        Singers = @event.Singers;
        Version = @event.Version;
    }

    public void Apply(RateSongEvent @event)
    {
        Rating = @event.Rating;
        Version = @event.Version;
    }

    public void Apply(LyricAddedEvent @event)
    {
        Lyrics.Add(new Lyric(@event.Value));
        Version = @event.Version;
    }
}