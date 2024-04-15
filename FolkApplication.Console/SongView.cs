using FolkApplication.Framework;

namespace FolkApplication.Console;

public record SongView : AggregateView
{
    public string Name { get; set; }
    public int Rating { get; set; }
}