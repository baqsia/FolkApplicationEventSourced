using FolkApplication.Framework;
using MongoDB.Driver;

namespace FolkApplication.ViewContext;

public class DomainViewRepository(IMongoDatabase mongoDatabase) : IDomainViewRepository
{
    private const string CollectionName = "songs";

    public async Task<IEnumerable<TAggregateView>?> QueryAsync<TAggregateView>(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default
    ) where TAggregateView : AggregateView
    {
        var filter = Builders<TAggregateView>.Filter.Empty;
        var result = await mongoDatabase.GetCollection<TAggregateView>(CollectionName)
            .Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return result;
    }

    public async Task<bool> PushAsync<TAggregateView>(
        TAggregateView aggregate,
        CancellationToken cancellation
    ) where TAggregateView : AggregateView
    {
        await mongoDatabase.GetCollection<TAggregateView>(CollectionName)
            .InsertOneAsync(aggregate, cancellationToken: cancellation);
        return true;
    }

    public async Task<bool> UpdateAsync<TAggregateView>(
        TAggregateView aggregate,
        CancellationToken cancellation
    ) where TAggregateView : AggregateView
    {
        var filter = Builders<TAggregateView>
            .Filter
            .Eq(a => a.Id, aggregate.Id);

        await mongoDatabase.GetCollection<TAggregateView>(CollectionName)
            .ReplaceOneAsync(filter, aggregate, cancellationToken: cancellation);
        return true;
    }

    public async Task<TAggregateView?> QueryFirstAsync<TAggregateView>(
        Guid id,
        CancellationToken cancellationToken
    ) where TAggregateView : AggregateView
    {
        var filter = Builders<TAggregateView>.Filter
            .Eq(aggregate => aggregate.Id, id.ToString());

        var result = await mongoDatabase.GetCollection<TAggregateView>(CollectionName)
            .FindAsync(filter, cancellationToken: cancellationToken);
        return await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}


// public class DomainViewRepository(ElasticsearchClient elasticClient) : IDomainViewRepository
// {
//     public async Task<IEnumerable<TAggregateView>?> QueryAsync<TAggregateView>(int pageNumber = 1, int pageSize = 20,
//         CancellationToken cancellationToken = default)
//         where TAggregateView : AggregateView
//     {
//         var result = await elasticClient.SearchAsync<TAggregateView>(ser =>
//         {
//             ser.Index(ElasticIndices.SongIndex)
//                 .From((pageNumber - 1) * 20)
//                 .Size(pageSize);
//         }, cancellationToken);
//
//         return result.Documents;
//     }
//
//     public async Task<bool> PushAsync<TAggregateView>(TAggregateView aggregate, CancellationToken cancellation)
//     {
//         var result = await elasticClient.IndexAsync(new IndexRequest<TAggregateView>(ElasticIndices.SongIndex)
//         {
//             Document = aggregate
//         }, cancellation);
//         return result.Result == Result.Created;
//     }
//
//     public async Task<bool> UpdateAsync<TAggregateView>(TAggregateView aggregate, CancellationToken cancellation)
//         where TAggregateView : AggregateView
//     {
//         var result = await elasticClient.UpdateAsync<TAggregateView, TAggregateView>(
//             ElasticIndices.SongIndex, 
//             aggregate.Id,
//             u => u.Doc(aggregate),
//             cancellation
//         );
//         return result.Result == Result.Updated;
//     }
//
//     public async Task<TAggregateView?> QueryFirstAsync<TAggregateView>(Guid id,
//         CancellationToken cancellationToken)
//         where TAggregateView : AggregateView
//     {
//         var result = await elasticClient.SearchAsync<TAggregateView>(ser =>
//         {
//             ser.Index(ElasticIndices.SongIndex)
//                 .Query(q =>
//                     q.Bool(b =>
//                         b.Filter(
//                             fq => fq.Term(t => t.Field("id.keyword").Value(id.ToString()))
//                         )
//                     )
//                 );
//         }, cancellationToken);
//
//         return result.Documents.FirstOrDefault();
//     }
// }