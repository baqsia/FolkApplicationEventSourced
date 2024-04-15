using Elastic.Clients.Elasticsearch;
using FolkApplication.Framework;

namespace FolkApplication.ViewContext;

public class DomainViewRepository(ElasticsearchClient elasticClient) : IDomainViewRepository
{
    public async Task<IEnumerable<TAggregateView>?> QueryAsync<TAggregateView>(int pageNumber = 1, int pageSize = 20,
        CancellationToken cancellationToken = default)
        where TAggregateView : AggregateView
    {
        var result = await elasticClient.SearchAsync<TAggregateView>(ser =>
        {
            ser.Index(ElasticIndices.SongIndex)
                .From((pageNumber - 1) * 20)
                .Size(pageSize);
        }, cancellationToken);

        return result.Documents;
    }

    public async Task<bool> PushAsync<TAggregateView>(TAggregateView aggregate, CancellationToken cancellation)
    {
        var result = await elasticClient.IndexAsync(new IndexRequest<TAggregateView>(ElasticIndices.SongIndex)
        {
            Document = aggregate
        }, cancellation);
        return result.Result == Result.Created;
    }

    public async Task<bool> UpdateAsync<TAggregateView>(TAggregateView aggregate, CancellationToken cancellation)
        where TAggregateView : AggregateView
    {
        var result = await elasticClient.UpdateAsync<TAggregateView, TAggregateView>(
            ElasticIndices.SongIndex, 
            aggregate.Id,
            u => u.Doc(aggregate),
            cancellation
        );
        return result.Result == Result.Updated;
    }

    public async Task<TAggregateView?> QueryFirstAsync<TAggregateView>(Guid id,
        CancellationToken cancellationToken)
        where TAggregateView : AggregateView
    {
        var result = await elasticClient.SearchAsync<TAggregateView>(ser =>
        {
            ser.Index(ElasticIndices.SongIndex)
                .Query(q =>
                    q.Bool(b =>
                        b.Filter(
                            fq => fq.Term(t => t.Field("id.keyword").Value(id.ToString()))
                        )
                    )
                );
        }, cancellationToken);

        return result.Documents.FirstOrDefault();
    }
}