namespace FolkApplication.Console;

public class SingerView
{
    public required string Name { get; set; }
    public bool IsBand { get; set; }
    public List<SongView> Songs { get; set; } = [];
}