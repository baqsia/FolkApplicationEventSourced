using FolkApplication.Domain.Song;
using FolkApplication.Framework;
using Marten;
using Marten.Events;
using Marten.Events.Projections;

namespace FolkApplication.Console;

public class SongProjectionViewStream(IDomainViewRepository viewRepository) : IProjection
{
    private async Task CopyAsDocument(StreamAction stream, CancellationToken cancellation)
    {
        var aggregate = new Song();
        foreach (var @event in stream.Events)
        {
            aggregate.Apply((dynamic)@event.Data);
            aggregate.Version = @event.Version;
        }

        await viewRepository.PushAsync(aggregate, cancellation);
    }
    
    private async Task CopyAsDocument(Song view, StreamAction stream, CancellationToken cancellation)
    {
        foreach (var @event in stream.Events)
        {
            view.Apply((dynamic)@event.Data);
            view.Version = @event.Version;
        }

        await viewRepository.PushAsync(view, cancellation);
    }

    public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        throw new NotImplementedException();
    }

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams,
        CancellationToken cancellation)
    {
        foreach (var stream in streams)
        {
            var view = await viewRepository.QueryFirstAsync<Song>(stream.Id, stream.Version - 1,
                cancellationToken: cancellation);
            if (view is not null)
            {
                if (view.Version < stream.Version)
                    await CopyAsDocument(view, stream, cancellation);

                continue;
            }

            await CopyAsDocument(stream, cancellation);
        }
    }
}