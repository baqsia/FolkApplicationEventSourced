using Marten.Events;
using Marten.Pagination;

namespace FolkApplication.Framework;

public interface IDomainEventRepository
{
    Task<Guid> Store<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken)
        where TAggregate : AggregateRoot;

    Task<IReadOnlyList<IEvent>> StreamByIdAsync(Guid tid, long version, CancellationToken cancellationToken);

    Task<TAggregate?> FindProjection<TAggregate>(Guid id, long version, CancellationToken cancellationToken)
        where TAggregate : AggregateRoot;
}