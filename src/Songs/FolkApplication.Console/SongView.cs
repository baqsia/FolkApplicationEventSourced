using FolkApplication.Domain.Song;
using FolkApplication.Framework;

namespace FolkApplication.Console;

public record SongView : AggregateView
{
    public string? Name { get; set; }
    public int Rating { get; set; }
    public List<Lyric> Lyrics { get; init; } = [];
    public List<SingerView> Singers { get; set; } = [];
}