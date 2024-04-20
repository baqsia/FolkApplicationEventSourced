namespace FolkApplication.Framework;

public interface IDomainViewRepository
{
    Task<IEnumerable<TAggregateView>?> QueryAsync<TAggregateView>(int pageNumber = 1, int pageSize = 20,
        CancellationToken cancellationToken = default) where TAggregateView : AggregateView;

    Task<bool> PushAsync<TAggregateView>(TAggregateView aggregate, CancellationToken cancellation)
        where TAggregateView : AggregateView;

    Task<bool> UpdateAsync<TAggregateView>(TAggregateView aggregate, CancellationToken cancellation)
        where TAggregateView : AggregateView;

    Task<TAggregateView?> QueryFirstAsync<TAggregateView>(Guid id,
        CancellationToken cancellationToken)
        where TAggregateView : AggregateView;
}