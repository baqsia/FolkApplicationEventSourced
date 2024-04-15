namespace FolkApplication.Framework;

public interface IDomainViewRepository
{
    Task<IEnumerable<TAggregate>?> QueryAsync<TAggregate>(int pageNumber = 1, int pageSize = 20,
        CancellationToken cancellationToken = default) where TAggregate : AggregateRoot;

    Task<bool> PushAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellation);

    Task<TAggregate?> QueryFirstAsync<TAggregate>(Guid streamId, long streamVersion,
        CancellationToken cancellationToken);
}