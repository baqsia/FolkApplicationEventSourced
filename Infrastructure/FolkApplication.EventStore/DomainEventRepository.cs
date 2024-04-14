using FolkApplication.Framework;
using Marten;
using Marten.Events;
using Marten.Pagination;

namespace FolkApplication.Infrastructure;

public class DomainEventRepository(IDocumentStore documentSession) : IDomainEventRepository
{
    public async Task<Guid> Store<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken)
        where TAggregate : AggregateRoot
    {
        await using var session = documentSession.LightweightSession();
        var action = session.Events.StartStream<TAggregate>(aggregate.Id, aggregate.Events());
        aggregate.Version = action.Version;
        await session.SaveChangesAsync(cancellationToken);
        return action.Id;
    }

    public async Task<IReadOnlyList<IEvent>> StreamByIdAsync(Guid tid, long version, CancellationToken cancellationToken)
    {
        await using var session = documentSession.LightweightSession();
        return await session.Events.FetchStreamAsync(tid, version, token: cancellationToken);
    }
    
    public async Task<TAggregate?> FindProjection<TAggregate>(Guid id, long version, CancellationToken cancellationToken)
        where TAggregate : AggregateRoot
    {
        await using var session = documentSession.LightweightSession();
        return await session.Events.AggregateStreamAsync<TAggregate>(id, token: cancellationToken);
    }

    public async Task<IPagedList<TAggregate>?> QueryPaged<TAggregate>(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default) where TAggregate : AggregateRoot
    {
        await using var session = documentSession.QuerySession();
        return await session.Query<TAggregate>()
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }
}