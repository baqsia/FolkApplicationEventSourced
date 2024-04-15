using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MGet;
using FolkApplication.Framework;

namespace FolkApplication.ViewContext;

public class DomainViewRepository(ElasticsearchClient elasticClient) : IDomainViewRepository
{
    public async Task<IEnumerable<TAggregate>?> QueryAsync<TAggregate>(int pageNumber = 1, int pageSize = 20,
        CancellationToken cancellationToken = default)
        where TAggregate : AggregateRoot
    {
        var result = await elasticClient.SearchAsync<TAggregate>(ser =>
        {
            ser.Index(ElasticIndices.SongIndex)
                .From((pageNumber - 1) * 20)
                .Size(pageSize);
        }, cancellationToken);

        return result.Documents;
    }

    public async Task<bool> PushAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellation)
    {
        var result = await elasticClient.IndexAsync(new IndexRequest<TAggregate>(ElasticIndices.SongIndex)
        {
            Document = aggregate
        }, cancellation);
        return result.Result == Result.Created;
    }

    public async Task<TAggregate?> QueryFirstAsync<TAggregate>(Guid streamId, long streamVersion,
        CancellationToken cancellationToken)
    {
        var result =
            await elasticClient.SearchAsync<TAggregate>(s =>
            {
                s.Index(ElasticIndices.SongIndex)
                    .Query(q =>
                    {
                        q.Bool(b =>
                            b.Filter(f =>
                                f.Term(t =>
                                    t.Field("id").Value(streamId.ToString())
                                ).Term(t =>
                                    t.Field("version").Value(streamVersion))
                            )
                        );
                    });
            }, cancellationToken);

        return result.Documents.FirstOrDefault();
    }
}