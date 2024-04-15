namespace FolkApplication.Framework;

public class AggregateRoot
{
    private readonly ICollection<DomainEvent> _events = [];
    public DomainEvent[] Events() => _events.ToArray();
    
    public Guid Id { get; protected set; } = default!;
    public long Version { get; set; }

    protected void Event(DomainEvent domainEvent) => _events.Add(domainEvent);
}