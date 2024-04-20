namespace FolkApplication.Framework;

public record AggregateView
{
    public string Id { get; init; } = default!;
    public long Version { get; set; }
}