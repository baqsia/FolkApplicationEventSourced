namespace FolkApplication.Framework;

public record AggregateView
{
    public Guid Id { get; set; }
    public int Version { get; set; }
}