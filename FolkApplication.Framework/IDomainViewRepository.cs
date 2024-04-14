namespace FolkApplication.Framework;

public interface IDomainViewRepository
{
    Task<IEnumerable<TAggregate>?> QueryPaged<TAggregate>(int pageNumber = 1, int pageSize = 20,
        CancellationToken cancellationToken = default) where TAggregate : AggregateRoot;
}