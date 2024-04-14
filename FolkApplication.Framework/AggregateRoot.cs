namespace FolkApplication.Framework;

public class AggregateRoot
{
    private readonly ICollection<DomainEvent> _events = [];
    public IEnumerable<DomainEvent> Events() => _events.ToList();
    
    public Guid Id { get; set; } = default!;
    public long Version { get; set; }

    protected void Event(DomainEvent domainEvent) => _events.Add(domainEvent);
}